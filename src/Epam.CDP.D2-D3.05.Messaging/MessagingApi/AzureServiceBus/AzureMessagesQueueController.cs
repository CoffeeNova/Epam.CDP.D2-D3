using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace MessagingApi.AzureServiceBus
{
    public class AzureMessagesQueueController : AzureMessagesControllerBase
    {
        public AzureMessagesQueueController(IServiceBusConfiguration configuration) : base(configuration)
        {
        }

        public override async Task<bool> QueueExist()
        {
            var manager = GetNamespaceManager();
            return await manager.QueueExistsAsync(Configuration.TopicName);
        }

        public override async Task CreateQueueAsync()
        {
            var manager = GetNamespaceManager();
            if (!await manager.QueueExistsAsync(Configuration.QueueName))
            {
                var queueDescription = new QueueDescription(Configuration.QueueName)
                {
                    DefaultMessageTimeToLive = new TimeSpan(0, 0, MessageTtl)
                };
                await manager.CreateQueueAsync(queueDescription);
            }
        }

        public override async Task DeleteQueueAsync()
        {
            var manager = GetNamespaceManager();
            if (await manager.QueueExistsAsync(Configuration.QueueName))
            {
                await manager.DeleteQueueAsync(Configuration.QueueName);
            }
        }

        public override async Task SendMessagesAsync<TMessage>(TMessage[] messages)
        {
            var queueClient = GetQueueClient();
            foreach (var message in messages)
            {
                await queueClient.SendAsync(new BrokeredMessage(JsonConvert.SerializeObject(message)));
            }

            await queueClient.CloseAsync();
        }

        public override async Task<TMessage> RecieveMessageAsync<TMessage>()
        {
            var queueClient = GetQueueClient();
            var result = await queueClient.ReceiveAsync();
            var body = result.GetBody<TMessage>();
            await queueClient.CloseAsync();

            return body;
        }

        private CancellationTokenSource _cts;
        public override async Task SubscribeToQueueAsync<TMessage>(Func<TMessage, Task> messageHandler)
        {
            _cts = new CancellationTokenSource();
            while (!_cts.IsCancellationRequested)
            {
                var message = await RecieveMessageAsync<TMessage>();
                if (message != null)
                {
                    await messageHandler(message);
                }
            }
        }

        public override Task UnSubscribeFromQueueAsync()
        {
            _cts.Cancel();
            _cts.Dispose();

            return Task.FromResult(0);
        }

        private QueueClient GetQueueClient() => QueueClient.CreateFromConnectionString(Configuration.ConnectionString, Configuration.QueueName, ReceiveMode.ReceiveAndDelete);
    }
}
