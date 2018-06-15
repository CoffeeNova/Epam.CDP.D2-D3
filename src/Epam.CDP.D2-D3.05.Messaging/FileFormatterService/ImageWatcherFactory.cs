namespace FileFormatterService
{
    internal class ImageWatcherFactory : IImageWatcherFactory
    {
        public void Create(out IImageWatcher watcher, EndOfFileEventHandler endOfFileEventHandler)
        {
            ImageWatcherHelper.CreateImageWatcher(out watcher, endOfFileEventHandler);
        }
    }
}
