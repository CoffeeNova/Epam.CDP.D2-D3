using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Common;
using FileFormatterService.Exceptions;
using MessagingApi;

namespace FileFormatterService
{
    public class FileFormatterService
    {
        private ImageWatcher _imageWatcher;
        private ServiceState _state;
        private readonly IFileBuilderFactory _fileBuilderFactory;
        private readonly IMessagesController _messageController;

        public FileFormatterService(IFileBuilderFactory fileBuilderFactory, IMessagesController messageController)
        {
            _fileBuilderFactory = fileBuilderFactory;
            _messageController = messageController;

#if DEBUG
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var imagesPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "ImagesReceiver"));
            MonitoringPaths.Add(imagesPath);
            DamagedPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "Damaged"));
            FileType = FileType.Pdf;
            NewPageTimeOut = 4000;
#endif
        }

        public void Start()
        {
            if (_state == ServiceState.Started)
                return;

            CheckDirectoriesForNewData();
            ImageWatcherHelper.CreateImageWatcher(out _imageWatcher, ImageExtensions.ToList(), MonitoringPaths, NewPageTimeOut, _imageWatcher_EndOfFileEventDetected);
            _state = ServiceState.Started;
        }

        public void Stop()
        {
            if (_state == ServiceState.Stopped)
                return;

            ImageWatcherHelper.DisposeImageWatcher(ref _imageWatcher, _imageWatcher_EndOfFileEventDetected);
            _state = ServiceState.Stopped;
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
                        BuildFile(sourceFiles, stream, out string fileName);
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
            var fileAlias = Path.GetFileNameWithoutExtension(imagesPaths.First())?.Split('_').First();
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
            var messageSequence = MessageItem.BuildFileMessageSequence(stream, fileName, MaxMessagesSize);
            _messageController.SendMessagesAsync(messageSequence).GetAwaiter().GetResult();
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

        public static ICollection<string> MonitoringPaths { get; set; } = new List<string>();

        public static int NewPageTimeOut { get; set; } = 10000;

        public static FileType FileType { get; set; }

        public static string DamagedPath { get; set; }

        public static int AttemptCount { get; set; } = 3;

        /// <summary>
        /// Max messages size in bytes
        /// </summary>
        public static int MaxMessagesSize { get; set; } = 64 * 1024;

        private enum ServiceState
        {
            Stopped,
            Started
        }

        private string[] ImageExtensions => new[] { Constants.FileExtension.Jpg, Constants.FileExtension.Png };
    }
}
