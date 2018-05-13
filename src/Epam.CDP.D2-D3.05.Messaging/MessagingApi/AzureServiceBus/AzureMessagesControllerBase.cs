using System;
using System.Threading.Tasks;

namespace MessagingApi.AzureServiceBus
{
    public abstract class AzureMessagesControllerBase : IMessagesController
    {
        protected readonly IServiceBusConfiguration Configuration;

        protected AzureMessagesControllerBase(IServiceBusConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public abstract Task<bool> QueueExist();
        public abstract Task CreateQueueAsync();
        public abstract Task DeleteQueueAsync();
        public abstract Task SendMessagesAsync<TMessage>(TMessage[] messages) where TMessage : MessageItem;
        public abstract Task<TMessage> RecieveMessageAsync<TMessage>() where TMessage : MessageItem;
        public abstract Task SubscribeToQueueAsync<TMessage>(Func<TMessage, Task> messageHandler) where TMessage : MessageItem;
        public abstract Task UnSubscribeFromQueueAsync();

        public virtual int MessageTtl { get; set; } = 60;
    }
}
