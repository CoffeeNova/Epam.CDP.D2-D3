using System;
using System.Collections.Generic;

namespace FileFormatterService
{
    public interface IImageWatcher : IDisposable
    {
        void WatchDirectories(ICollection<string> monitoringPaths, ICollection<string> extensions, int newPageTimeout = 10);
        void CheckDirectoriesForNewImages();
        event EndOfFileEventHandler EndOfFileEventDetected;
        ICollection<string> ImageExtensions { get; set; }
        int NewPageTimeout { get; set; }
    }
}