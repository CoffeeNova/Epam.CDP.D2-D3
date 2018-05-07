using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Common;

namespace FileFormatterService
{
    public class FileFormatterService
    {
        private ImageWatcher _imageWatcher;
        private ServiceState _state;

        public FileFormatterService()
        {
#if DEBUG
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var imagesPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "ImagesReceiver"));
            MonitoringPaths.Add(imagesPath);
            OutputPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "Output"));
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
            var imagesPath = new List<string>();
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
                imagesPath.AddRange(files.Select(x => x.FullName));
            }

            if (imagesPath.Any())
            {
                var imgArr = imagesPath.ToArray();
                try
                {
                    BuildFile(imgArr);
                    CleanFiles(imgArr);
                }
                catch (InvalidOperationException)
                {
                    TryMoveFilesToDamagedFolder(imagesPath);
                }
            }
        }

        private void BuildFile(string[] imagesPaths)
        {
            if (!imagesPaths.Any())
                return;

            var fileBuilder = FileBuilderFactory.GetFileBuilder(FileType, OutputPath, imagesPaths);
            var fileName = Path.GetFileNameWithoutExtension(imagesPaths.First())?.Split('_').First();

            int attempt = AttemptCount;
            while (attempt > 0)
            {
                try
                {
                    fileBuilder.Build(fileName);
                    attempt = 0;
                }
                catch (Exception)
                {
                    if (attempt == 0)
                        throw new InvalidOperationException();

                    attempt--;
                    Thread.Sleep(2000);
                }
            }
        }

        private void CleanFiles(string[] imagesPaths)
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

        private void _imageWatcher_EndOfFileEventDetected(string[] imagesPaths)
        {
            try
            {
                BuildFile(imagesPaths);
                CleanFiles(imagesPaths);
            }
            catch (InvalidOperationException)
            {
                TryMoveFilesToDamagedFolder(imagesPaths);
            }
        }

        public static ICollection<string> MonitoringPaths { get; set; } = new List<string>();

        public static int NewPageTimeOut { get; set; } = 10000;

        public static FileType FileType { get; set; }

        public static string OutputPath { get; set; }

        public static string DamagedPath { get; set; }

        public static int AttemptCount { get; set; } = 3;

        private enum ServiceState
        {
            Stopped,
            Started
        }

        private string[] ImageExtensions => new[] { Constants.FileExtension.Jpg, Constants.FileExtension.Png };
    }
}
