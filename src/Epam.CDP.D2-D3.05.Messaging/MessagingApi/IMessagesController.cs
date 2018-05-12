using System.Threading.Tasks;

namespace MessagingApi
{
    public interface IMessagesController
    {
        Task CreateQueue();
        Task SendMessagesAsync(MessageItem[] messages);
        Task<MessageItem> RecieveMessageAsync();
        int MessageTtl { get; set; }
    }
}
