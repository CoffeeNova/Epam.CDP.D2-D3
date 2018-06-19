using FileFormatter.Common;

namespace FileFormatterService
{
    internal class ImageWatcherFactory : IImageWatcherFactory
    {
        [UniversalOnMethodBoundaryAspect]
        public void Create(out IImageWatcher watcher, EndOfFileEventHandler endOfFileEventHandler)
        {
            ImageWatcherHelper.CreateImageWatcher(out watcher, endOfFileEventHandler);
        }
    }
}
