using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace MessagingApi.AzureServiceBus
{
    public class AzureMessagesController : IMessagesController
    {
        private readonly ServiceBusConfiguration _configuration;

        public AzureMessagesController(ServiceBusConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task CreateQueue()
        {
            var manager = NamespaceManager.CreateFromConnectionString(_configuration.ConnectionString);
            if (!await manager.QueueExistsAsync(_configuration.QueueName))
            {
                var queueDescription = new QueueDescription(_configuration.QueueName);
                await manager.CreateQueueAsync(queueDescription);
            }
        }

        public async Task SendMessagesAsync(MessageItem[] messages)
        {
            var queueClient = QueueClient.CreateFromConnectionString(_configuration.ConnectionString, _configuration.QueueName, ReceiveMode.ReceiveAndDelete);
            await queueClient.SendBatchAsync(messages.Select(x => new BrokeredMessage(x)));
            await queueClient.CloseAsync();
        }

        public async Task<MessageItem[]> RecieveMessagesAsync(int messagesCount)
        {
            var queueClient = QueueClient.CreateFromConnectionString(_configuration.ConnectionString, _configuration.QueueName, ReceiveMode.ReceiveAndDelete);
            var result = await queueClient.ReceiveBatchAsync(messagesCount);
            await queueClient.CloseAsync();
            return result.Select(x => x.GetBody<MessageItem>()).ToArray();
        }
    }
}
