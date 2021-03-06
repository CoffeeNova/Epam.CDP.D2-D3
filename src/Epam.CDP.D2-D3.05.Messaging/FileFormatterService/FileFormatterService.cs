﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileFormatter.Common;
using FileFormatter.Common.ServiceBusConfiguration;
using FileFormatterService.Exceptions;
using MessagingApi.AzureServiceBus;
using Topshelf;

namespace FileFormatterService
{
    internal class FileFormatterService : IFileFormatterService
    {
        private ServiceStatus _status;
        private readonly IFileBuilderFactory _fileBuilderFactory;
        private readonly IFileFormatterSettingsExchanger _settingsExchanger;
        private readonly IImageWatcherFactory _imageWatcherFactory;

        private HostControl _hostControl;
        private readonly IServiceBusConfiguration _fileQueueConfig;
        private readonly IServiceBusConfiguration _statusQueueConfig;
        private readonly IServiceBusConfiguration _controlQueueConfig;
        private IImageWatcher _imageWatcher;

        public FileFormatterService(
            IFileBuilderFactory fileBuilderFactory,
            IServiceBusConfigurationFactory serviceBusConfigurationFactory,
            IFileFormatterSettingsExchanger settingsExchanger,
            IImageWatcherFactory imageWatcherFactrory)
        {
            _fileBuilderFactory = fileBuilderFactory;
            _fileQueueConfig = serviceBusConfigurationFactory.CreateByType(SbConfigType.FileQueue);
            _statusQueueConfig = serviceBusConfigurationFactory.CreateByType(SbConfigType.StatusQueue);
            _controlQueueConfig = serviceBusConfigurationFactory.CreateByType(SbConfigType.ControlQueue);
            _controlQueueConfig.SubscriptionName = NodeName;

            _settingsExchanger = settingsExchanger;
            _imageWatcherFactory = imageWatcherFactrory;

#if DEBUG
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var imagesPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "ImagesReceiver"));
            MonitoringPaths.Add(imagesPath);
            DamagedPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "Damaged"));
            FileType = FileType.Pdf;
            NewPageTimeOut = 4000;
#endif

        }

        [UniversalOnMethodBoundaryAspect]
        public bool Start(HostControl hostControl)
        {
            _hostControl = hostControl;

            if (_status != ServiceStatus.Stopped)
                return true;
            try
            {
                _imageWatcherFactory.Create(out _imageWatcher, _imageWatcher_EndOfFileEventDetected);
                _imageWatcher.WatchDirectories(MonitoringPaths, ImageExtensions, NewPageTimeOut);

                _settingsExchanger.SubscribeToSettingsSender(GetCurrentServiceSettings, _statusQueueConfig);
                _settingsExchanger.SubscribeToSettingsReceiver(OnReceiveNewSettings, _controlQueueConfig);
            }
            catch
            {
                //log
                hostControl.Stop();
                return true;
            }

            _status = ServiceStatus.Waiting;

            return true;
        }

        [UniversalOnMethodBoundaryAspect]
        public void Stop()
        {
            if (_status != ServiceStatus.Waiting)
                return;

            ImageWatcherHelper.DisposeImageWatcher(ref _imageWatcher, _imageWatcher_EndOfFileEventDetected);
            _settingsExchanger.UnSubscribeFromSettingsSender();
            _settingsExchanger.UnSubscribeFromSettingsReceiver();

            _status = ServiceStatus.Stopped;
        }

        private void TryBuildAndSendFilesWithAttempt(string[] sourceFiles)
        {
            var actionResult = TryDoActionWithAttempts(() =>
             {
                 using (var stream = new MemoryStream())
                 {
                     BuildFile(sourceFiles, stream, out var fileName);
                     SendFileToCentralServer(stream, fileName);
                 }
             }, out var exception);
            if (!actionResult && exception is BuildFileException)
            {
                TryDoActionWithAttempts(() =>
                {
                    TryMoveFilesToDamagedFolder(sourceFiles);
                }, out exception);
            }
            else
                TryDoActionWithAttempts(() =>
                {
                    CleanSourceFiles(sourceFiles);
                }, out exception);
        }

        private bool TryDoActionWithAttempts(Action action, out Exception exception)
        {
            var attempt = AttemptCount;
            Exception localEx = null;
            while (attempt > 0)
            {
                try
                {
                    action();
                    exception = null;
                    return true;
                }
                catch (Exception ex)
                {
                    attempt--;
                    Thread.Sleep(2000);
                    if (attempt == 0)
                        localEx = ex;
                }
            }

            exception = localEx;
            return false;
        }

        private void BuildFile(string[] imagesPaths, MemoryStream stream, out string fileName)
        {
            fileName = null;
            if (!imagesPaths.Any())
                return;

            var fileBuilder = _fileBuilderFactory.GetFileBuilder(FileType, imagesPaths);
            var fileAlias = BuildFileAlias(imagesPaths.First());
            try
            {
                fileBuilder.Build(fileAlias, stream);
                fileName = fileBuilder.FileName;
            }
            catch (Exception ex)
            {
                throw new BuildFileException("Can't build file. Probably file is damaged.", ex);
            }
        }

        private void SendFileToCentralServer(MemoryStream stream, string fileName)
        {
            var messageSequence = FileMessageSequence.BuildFileMessageItems(stream, fileName, MaxMessagesSize);
            var fileMessagesController = new AzureMessagesQueueController(_fileQueueConfig);
            fileMessagesController.SendMessagesAsync(messageSequence).GetAwaiter().GetResult();
        }

        private void CleanSourceFiles(string[] imagesPaths)
        {
            foreach (var imagePath in imagesPaths)
            {
                var file = new FileInfo(imagePath);
                if (file.Exists)
                    file.Delete();
            }
        }

        private void TryMoveFilesToDamagedFolder(ICollection<string> imagesPaths)
        {
            var subFolderPath = Path.Combine(DamagedPath, DateTime.Now.ToString("dd-MM-yy hh.mm.ss"));
            if (!Directory.Exists(subFolderPath))
                Directory.CreateDirectory(subFolderPath);

            foreach (var imagePath in imagesPaths)
            {
                try
                {
                    var file = new FileInfo(imagePath);
                    if (file.Exists)
                    {
                        file.MoveTo(Path.Combine(subFolderPath, file.Name));
                    }
                }
                catch (Exception)
                {
                    // to log
                }
            }
        }

        private void _imageWatcher_EndOfFileEventDetected(string[] sourceFilesPaths)
        {
            TryBuildAndSendFilesWithAttempt(sourceFilesPaths);
        }

        private string BuildFileAlias(string imagePath)
        {
            var sb = new StringBuilder(NodeName);
            sb.Append("_");
            sb.Append(Path.GetFileNameWithoutExtension(imagePath)?.Split('_').First());

            return sb.ToString();
        }

        private FileFormatterSettings GetCurrentServiceSettings()
        {
            return new FileFormatterSettings
            {
                ServiceStatus = ServiceController.GetServices().FirstOrDefault(x => string.Equals(x.ServiceName, nameof(FileFormatterService)))?.Status,
                NewPageTimeOut = NewPageTimeOut,
                NodeName = NodeName,
                Date = DateTime.Now
            };
        }

        private Task OnReceiveNewSettings(FileFormatterSettings settings)
        {
            if (settings.NewPageTimeOut.HasValue)
            {
                NewPageTimeOut = settings.NewPageTimeOut.Value;
                _imageWatcher.NewPageTimeout = settings.NewPageTimeOut.Value;
            }
            //here we can save this settings to the registry by path Computer\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\FileFormatterService

            if (settings.ServiceStatus.HasValue)
                ControlService(settings.ServiceStatus.Value);

            return Task.FromResult<object>(null);
        }

        private void ControlService(ServiceControllerStatus newStatus)
        {
            switch (newStatus)
            {
                case ServiceControllerStatus.Paused:
                    Stop();
                    break;
                case ServiceControllerStatus.Stopped:
                    _hostControl.Stop();
                    break;
                case ServiceControllerStatus.Running: //this is impossible in this case xD
                    break;
            }
        }

        public ICollection<string> MonitoringPaths { get; set; } = new List<string>();

        public int NewPageTimeOut { get; set; }

        public FileType FileType { get; set; }

        public string DamagedPath { get; set; }

        public int AttemptCount { get; set; } = 3;

        /// <summary>
        /// Max messages size in bytes
        /// </summary>
        public int MaxMessagesSize { get; set; } = 64 * 1024;

        public string NodeName { get; set; } = "node1";

        private string[] ImageExtensions => new[] { Constants.FileExtension.Jpg, Constants.FileExtension.Png };
    }
}
