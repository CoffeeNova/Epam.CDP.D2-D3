using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
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
            var manager = NamespaceManager.CreateFromConnectionString(Configuration.ConnectionString);
            return await manager.TopicExistsAsync(Configuration.TopicName);
        }

        public override async Task CreateQueueAsync()
        {
            var manager = NamespaceManager.CreateFromConnectionString(Configuration.ConnectionString);
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
            var manager = NamespaceManager.CreateFromConnectionString(Configuration.ConnectionString);
            if (await manager.TopicExistsAsync(Configuration.TopicName))
            {
                await manager.DeleteTopicAsync(Configuration.TopicName);
            }
        }

        public override async Task SendMessagesAsync<TMessage>(TMessage[] messages)
        {
            var topicClient = TopicClient.CreateFromConnectionString(Configuration.ConnectionString, Configuration.TopicName);
            await topicClient.SendBatchAsync(messages.Select(x => new BrokeredMessage(JsonConvert.SerializeObject(x))));
            await topicClient.CloseAsync();
        }

        public override Task<TMessage> RecieveMessageAsync<TMessage>()
        {
            throw new NotImplementedException();
        }

        public override async Task SubscribeToQueueAsync<TMessage>(Func<TMessage, Task> messageHandler)
        {
            var manager = NamespaceManager.CreateFromConnectionString(Configuration.ConnectionString);
            if (!await manager.SubscriptionExistsAsync(Configuration.TopicName, Configuration.SubscriptionName))
            {
                var subscrDescr = new SubscriptionDescription(Configuration.TopicName, Configuration.SubscriptionName)
                {
                    DefaultMessageTimeToLive = new TimeSpan(0, 0, MessageTtl)
                };
                await manager.CreateSubscriptionAsync(subscrDescr);
            }

            var subscrClient = SubscriptionClient.Create(Configuration.TopicName, Configuration.SubscriptionName);
            subscrClient.OnMessageAsync(x =>
            {
                var body = x.GetBody<string>();
                var messageItem = JsonConvert.DeserializeObject<TMessage>(body);
                return messageHandler(messageItem);
            });

            await subscrClient.CloseAsync();
        }

        public override async Task UnSubscribeFromQueueAsync()
        {
            var manager = NamespaceManager.CreateFromConnectionString(Configuration.ConnectionString);
            if (await manager.SubscriptionExistsAsync(Configuration.TopicName, Configuration.SubscriptionName))
            {
                await manager.DeleteSubscriptionAsync(Configuration.TopicName, Configuration.SubscriptionName);
            }
        }
    }
}
