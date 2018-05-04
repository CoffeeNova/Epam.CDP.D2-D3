using System.Collections.Generic;
using System.IO;

namespace FileFormatterService
{
    internal static class ImageWatcherHelper
    {
        public static void WatcherSettings(this FileSystemWatcher watcher, FileSystemEventHandler onCreated, ErrorEventHandler onError)
        {
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.Created += onCreated;
            watcher.InternalBufferSize = 81920;
            watcher.EnableRaisingEvents = true;
            watcher.Error += onError;
        }

        public static void CreateImageWatcher(out ImageWatcher watcher, ICollection<string> imageExtensions, ICollection<string> monitoringPaths, int newPageTimeout, ImageWatcher.EndOfFileEventHandler endOfFileEventHandler)
        {
            watcher = new ImageWatcher(monitoringPaths, newPageTimeout)
            {
                ImageExtensions = imageExtensions
            };
            watcher.EndOfFileEventDetected += endOfFileEventHandler;
        }

        public static void DisposeImageWatcher(ref ImageWatcher watcher, ImageWatcher.EndOfFileEventHandler endOfFileEventHandler)
        {
            if (watcher == null)
                return;

            if (endOfFileEventHandler != null)
                watcher.EndOfFileEventDetected -= endOfFileEventHandler;
            watcher.Dispose();
        }

    }
}
