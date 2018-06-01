using MessagingApi;

namespace FileFormatterCentralService
{
    public interface IFileAssembler
    {
        void StartAssembling(string savePath, IMessagesController messageController);
        void StopAssembling();
    }
}
