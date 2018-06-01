using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using Common;

namespace FileFormatterService
{
    internal class ImageWatcher : IDisposable
    {
        private readonly List<string> _imagesNames;
        private readonly Timer _timer;
        private readonly List<FileSystemWatcher> _watchers;

        public ImageWatcher(ICollection<string> monitoringPaths, int newPageTimeout, ICollection<string> extensions)
        {
            ImageExtensions = extensions;
            NewPageTimeout = newPageTimeout;
            _imagesNames = new List<string>();
            _timer = new Timer
            {
                Interval = NewPageTimeout,
                AutoReset = false,
            };
            _timer.Elapsed += _timer_Elapsed;

            _watchers = new List<FileSystemWatcher>();
            foreach (var path in monitoringPaths)
            {
                if (!Directory.Exists(path))
                    throw new DirectoryNotFoundException($"Directory '{path}' was not found.");

                var watcher = new FileSystemWatcher(path);
                watcher.WatcherSettings(Watcher_Created, Watcher_Error);
                _watchers.Add(watcher);
            }
        }

        public void CheckDirectoriesForNewImages()
        {
            _watchers.ForEach(w =>
            {
                var dir = new DirectoryInfo(w.Path);
                var files = dir.GetFilesByExtensions(ImageExtensions?.ToArray())
                    .OrderBy(f => f.CreationTime) //order by creation time to reproduce queue
                    .ToList();

                foreach (var file in files)
                {
                    var fsEventArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, w.Path, file.Name);
                    Watcher_Created(this, fsEventArgs);
                }
            });
        }

        public delegate void EndOfFileEventHandler(string[] imagesPaths);

        public event EndOfFileEventHandler EndOfFileEventDetected;

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (!ImageExtensions.Any() || !ImageFilter(e.Name))
                return;

            lock (_locker)
            {
                if (_imagesNames.Any())
                {
                    var prevFileName = Path.GetFileNameWithoutExtension(_imagesNames.Last());
                    var newFileName = Path.GetFileNameWithoutExtension(e.Name);
                    if (IsNumericGapDetected(prevFileName, newFileName))
                    {
                        EndOfFileEventDetected?.Invoke(_imagesNames.ToArray());
                        _imagesNames.Clear();
                    }
                }

                _imagesNames.Add(e.FullPath);
                _timer.Stop();
                _timer.Start();
            }
        }

        private bool IsNumericGapDetected(string lastFileName, string newFileName)
        {
            var n1 = int.Parse(lastFileName.Split('_')[1]);
            var n2 = int.Parse(newFileName.Split('_')[1]);

            return n2 - n1 != 1;
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            //try to increase internal buffer
            if (sender is FileSystemWatcher fileSystemWatcher)
                fileSystemWatcher.InternalBufferSize = fileSystemWatcher.InternalBufferSize * 2;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_locker)
            {
                EndOfFileEventDetected?.Invoke(_imagesNames.ToArray());
                _imagesNames.Clear();
            }
        }

        private string ImageNamePattern => $@"\w+_\d+\.({string.Join("|", ImageExtensions)})";

        private bool ImageFilter(string fileName) => Regex.IsMatch(fileName, ImageNamePattern, RegexOptions.IgnoreCase);
        private readonly object _locker = new object();

        public ICollection<string> ImageExtensions { get; set; }
        public int NewPageTimeout { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
                _watchers.ForEach(x =>
                {
                    x.EnableRaisingEvents = false;
                    x.Created -= Watcher_Created;
                    x.Error -= Watcher_Error;
                    x.Dispose();
                });
            }
        }

        ~ImageWatcher()
        {
            Dispose(false);
        }
    }
}
