using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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
            _imageWatcher = new ImageWatcher(MonitoringPaths, NewPageTimeOut);
            _imageWatcher.EndOfFileEventDetected += _imageWatcher_EndOfFileEventDetected;
            _state = ServiceState.Started;
        }

        public void Stop()
        {
            if (_state == ServiceState.Stopped)
                return;

            _imageWatcher.EndOfFileEventDetected -= _imageWatcher_EndOfFileEventDetected;
            _imageWatcher.Dispose();
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
                var files = dir.GetFiles();
                imagesPath.AddRange(files.Select(x => x.FullName));
            }

            if (imagesPath.Any())
            {
                var imgArr = imagesPath.ToArray();
                BuildFile(imgArr);
                CleanFiles(imgArr);
            }
        }

        private void BuildFile(string[] imagesPaths)
        {
            if (!imagesPaths.Any())
                return;

            var fileBuilder = FileBuilderFactory.GetFileBuilder(FileType, OutputPath, imagesPaths);
            fileBuilder.FileName = Path.GetFileNameWithoutExtension(imagesPaths.First())?.Split('_').First();

            int attempt = AttemptCount;
            while (attempt > 0)
            {
                try
                {
                    fileBuilder.Build();
                    attempt = 0;
                }
                catch (Exception)
                {
                    attempt--;
                    Thread.Sleep(2000);
                }
                finally
                {
                    if (attempt == 0)
                        TryMoveFilesToDamagedFolder(imagesPaths);
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

        private void TryMoveFilesToDamagedFolder(string[] imagesPaths)
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
                catch (UnauthorizedAccessException ex)
                {
                    var message =
                        "Can't read source image file: '{imagePath}'. Probably file locked by another process.";
                    //var eventMessage =
                    //    message +
                    //    $"Inner message: {ex.Message}" +
                    //    $"Stack trace: {ex.StackTrace}";

                    //EventLog.WriteEntry(ServiceHelper.GetServiceName(), eventMessage, EventLogEntryType.Error, 121, 1);
                    throw new InvalidOperationException(message, ex);
                }
            }
        }

        private void _imageWatcher_EndOfFileEventDetected(string[] imagesPaths)
        {
            BuildFile(imagesPaths);
            CleanFiles(imagesPaths);
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
    }
}
