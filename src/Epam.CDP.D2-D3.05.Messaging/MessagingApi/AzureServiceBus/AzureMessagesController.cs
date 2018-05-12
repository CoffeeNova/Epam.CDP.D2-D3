using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

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
                var queueDescription = new QueueDescription(_configuration.QueueName)
                {
                    DefaultMessageTimeToLive = new TimeSpan(0, 0, MessageTtl)
                };
                await manager.CreateQueueAsync(queueDescription);
            }
        }

        public async Task SendMessagesAsync(MessageItem[] messages)
        {
            var queueClient = QueueClient.CreateFromConnectionString(_configuration.ConnectionString, _configuration.QueueName, ReceiveMode.ReceiveAndDelete);
            foreach (var message in messages)
            {
                await queueClient.SendAsync(new BrokeredMessage(JsonConvert.SerializeObject(message)));
            }
            await queueClient.CloseAsync();
        }

        public async Task<MessageItem> RecieveMessageAsync()
        {
            var queueClient = QueueClient.CreateFromConnectionString(_configuration.ConnectionString, _configuration.QueueName, ReceiveMode.ReceiveAndDelete);
            var result = await queueClient.ReceiveAsync();
            var body = result.GetBody<string>();
            await queueClient.CloseAsync();

            return JsonConvert.DeserializeObject<MessageItem>(body);
        }

        public int MessageTtl { get; set; } = 60;
    }
}
