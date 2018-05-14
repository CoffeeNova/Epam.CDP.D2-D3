using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using FileFormatter.Common;
using FileFormatterService.Exceptions;
using MessagingApi.AzureServiceBus;
using Topshelf;

namespace FileFormatterService
{
    public class FileFormatterService
    {
        private ImageWatcher _imageWatcher;
        private ServiceStatus _status;
        private readonly IFileBuilderFactory _fileBuilderFactory;
        private readonly IServiceBusConfiguration _fileQueueConfig;
        private readonly IServiceBusConfiguration _statusQueueConfig;
        private readonly IServiceBusConfiguration _controlQueueConfig;
        private readonly IFileFormatterSettingsExchanger _settingsExchanger;

        public FileFormatterService(IFileBuilderFactory fileBuilderFactory, IServiceBusConfiguration fileQueueConfig, IServiceBusConfiguration statusQueueConfig, IServiceBusConfiguration controlQueueConfig)
        {
            _fileBuilderFactory = fileBuilderFactory;
            _fileQueueConfig = fileQueueConfig;
            _statusQueueConfig = statusQueueConfig;
            _controlQueueConfig = controlQueueConfig;

            _controlQueueConfig.SubscriptionName = NodeName;

            _settingsExchanger = new FileFormatterSettingsExchanger();

#if DEBUG
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var imagesPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "ImagesReceiver"));
            MonitoringPaths.Add(imagesPath);
            DamagedPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "Damaged"));
            FileType = FileType.Pdf;
            NewPageTimeOut = 4000;
#endif
        }

        public bool Start(HostControl hostControl)
        {
            if (_status != ServiceStatus.Stopped)
                return true;
            try
            {
                CheckDirectoriesForNewData();
                ImageWatcherHelper.CreateImageWatcher(out _imageWatcher, ImageExtensions.ToList(), MonitoringPaths, NewPageTimeOut, _imageWatcher_EndOfFileEventDetected);

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

        public void Stop()
        {
            if (_status != ServiceStatus.Waiting)
                return;

            ImageWatcherHelper.DisposeImageWatcher(ref _imageWatcher, _imageWatcher_EndOfFileEventDetected);
            _settingsExchanger.UnSubscribeFromSettingsSender();
            _settingsExchanger.UnSubscribeFromSettingsReceiver();

            _status = ServiceStatus.Stopped;
        }

        private void CheckDirectoriesForNewData()
        {
            var sourceFilePaths = new List<string>();
            foreach (var path in MonitoringPaths)
            {
                if (!Directory.Exists(path))
                {
                    var message = $"Directory '{path}' was not found.";
                    //EventLog.WriteEntry(ServiceHelper.GetServiceName(), message, EventLogEntryType.Error);
                    throw new DirectoryNotFoundException(message);
                }

                var dir = new DirectoryInfo(path);
                var files = dir.GetFilesByExtensions(ImageExtensions);
                sourceFilePaths.AddRange(files.Select(x => x.FullName));
            }

            if (sourceFilePaths.Any())
            {
                TryBuildAndSendFilesWithAttempt(sourceFilePaths.ToArray());
            }
        }

        private void TryBuildAndSendFilesWithAttempt(string[] sourceFiles)
        {
            var attempt = AttemptCount;
            while (attempt > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        BuildFile(sourceFiles, stream, out var fileName);
                        SendFileToCentralServer(stream, fileName);
                        CleanSourceFiles(sourceFiles);
                    }
                    attempt = 0;
                }
                catch (BuildFileException)
                {
                    TryMoveFilesToDamagedFolder(sourceFiles);
                }
                catch (Exception)
                {
                    attempt--;
                    Thread.Sleep(2000);
                }
            }
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
            var messageSequence = FileMessageItem.BuildFileMessageSequence(stream, fileName, MaxMessagesSize);
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
                ServiceStatus = _status,
                NewPageTimeOut = NewPageTimeOut,
                NodeName = NodeName,
                Date = DateTime.Now
            };
        }

        private Task OnReceiveNewSettings(FileFormatterSettings settings)
        {
            if (settings.NewPageTimeOut.HasValue)
                NewPageTimeOut = settings.NewPageTimeOut.Value;
                //here we can save this settings to the registry by path Computer\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\FileFormatterService

            return Task.FromResult<object>(null);
        }

        public static ICollection<string> MonitoringPaths { get; set; } = new List<string>();

        public static int NewPageTimeOut { get; set; } = 10000;

        public static FileType FileType { get; set; }

        public static string DamagedPath { get; set; }

        public static int AttemptCount { get; set; } = 3;

        /// <summary>
        /// Max messages size in bytes
        /// </summary>
        public static int MaxMessagesSize { get; set; } = 64 * 1024;

        public static string NodeName { get; set; } = "node1";

        private string[] ImageExtensions => new[] { Constants.FileExtension.Jpg, Constants.FileExtension.Png };
    }
}
