using System;
using System.Threading.Tasks;

namespace MessagingApi
{
    public interface IMessagesController
    {
        Task<bool> QueueExist();
        Task CreateQueueAsync();
        Task DeleteQueueAsync();
        Task SendMessagesAsync<TMessage>(TMessage[] messages) where TMessage : MessageItem;
        Task<TMessage> RecieveMessageAsync<TMessage>() where TMessage : MessageItem;
        Task SubscribeToQueueAsync<TMessage>(Func<TMessage, Task> messageHandler) where TMessage : MessageItem;
        Task UnSubscribeFromQueueAsync();
        int MessageTtl { get; set; }
    }
}
