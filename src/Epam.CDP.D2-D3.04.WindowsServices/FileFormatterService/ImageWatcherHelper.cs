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
    }
}
