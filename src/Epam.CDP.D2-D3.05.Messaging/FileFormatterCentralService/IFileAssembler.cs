using MessagingApi;

namespace FileFormatterCentralService
{
    public interface IFileAssembler
    {
        void StartAssebling(string savePath, IMessagesController messageController);
        void StopAssebling();
    }
}
