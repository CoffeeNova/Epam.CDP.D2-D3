using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace MessagingApi.AzureServiceBus
{
    public class AzureMessagesTopicController : AzureMessagesControllerBase
    {
        public AzureMessagesTopicController(IServiceBusConfiguration configuration) : base(configuration)
        {
        }

        public override async Task<bool> QueueExist()
        {
            var manager = GetNamespaceManager();
            return await manager.TopicExistsAsync(Configuration.TopicName);
        }

        public override async Task CreateQueueAsync()
        {
            var manager = GetNamespaceManager();
            if (!await manager.TopicExistsAsync(Configuration.TopicName))
            {
                var topicDescription = new TopicDescription(Configuration.TopicName)
                {
                    DefaultMessageTimeToLive = new TimeSpan(0, 0, MessageTtl)
                };
                await manager.CreateTopicAsync(topicDescription);
            }
        }

        public override async Task DeleteQueueAsync()
        {
            var manager = GetNamespaceManager();
            if (await manager.TopicExistsAsync(Configuration.TopicName))
            {
                await manager.DeleteTopicAsync(Configuration.TopicName);
            }
        }

        public override async Task SendMessagesAsync<TMessage>(TMessage[] messages)
        {
            var topicClient = GetTopicClient();
            foreach (var message in messages)
            {
                await topicClient.SendAsync(new BrokeredMessage(JsonConvert.SerializeObject(message)));
            }

            await topicClient.CloseAsync();
        }

        public override Task<TMessage> RecieveMessageAsync<TMessage>()
        {
            throw new NotImplementedException();
        }

        private SubscriptionClient _subscrClient;
        public override async Task SubscribeToQueueAsync<TMessage>(Func<TMessage, Task> messageHandler)
        {
            if (_subscrClient != null && !_subscrClient.IsClosed)
                throw new InvalidOperationException("Already subscribed. Please unsubscribe first.");

            var manager = GetNamespaceManager();
            if (!await manager.SubscriptionExistsAsync(Configuration.TopicName, Configuration.SubscriptionName))
            {
                var subscrDescr = new SubscriptionDescription(Configuration.TopicName, Configuration.SubscriptionName)
                {
                    DefaultMessageTimeToLive = new TimeSpan(0, 0, MessageTtl)
                };
                await manager.CreateSubscriptionAsync(subscrDescr);
            }

            _subscrClient = SubscriptionClient.CreateFromConnectionString(Configuration.ConnectionString, Configuration.TopicName, Configuration.SubscriptionName);
            _subscrClient.OnMessageAsync(x =>
            {
                var body = x.GetBody<TMessage>();
                return messageHandler(body);
            });
        }

        public override async Task UnSubscribeFromQueueAsync()
        {
            if (_subscrClient != null && !_subscrClient.IsClosed)
                await _subscrClient.CloseAsync();

            var manager = GetNamespaceManager();
            if (await manager.SubscriptionExistsAsync(Configuration.TopicName, Configuration.SubscriptionName))
            {
                await manager.DeleteSubscriptionAsync(Configuration.TopicName, Configuration.SubscriptionName);
            }
        }

        private TopicClient GetTopicClient() => TopicClient.CreateFromConnectionString(Configuration.ConnectionString, Configuration.TopicName);
    }
}
