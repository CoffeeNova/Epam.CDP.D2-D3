namespace FileFormatterService
{
    public interface IImageWatcherFactory
    {
        void Create(out IImageWatcher watcher, EndOfFileEventHandler endOfFileEventHandler);
    }
}
