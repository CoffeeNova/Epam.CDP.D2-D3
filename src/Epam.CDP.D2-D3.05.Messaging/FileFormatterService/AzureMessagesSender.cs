using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace FileFormatterService
{
    public class AzureMessagesSender : IMessagesSender
    {
        private readonly ServiceBusConfiguration _configuration;
        private readonly NamespaceManager _namespaceManager;

        public AzureMessagesSender(ServiceBusConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _namespaceManager = NamespaceManager.CreateFromConnectionString(configuration.ConnectionString);
            if (!_namespaceManager.QueueExists(configuration.QueueName))
                throw new ArgumentException($"Queue with name '{configuration}' doesn't exist. Please create it first.");

            _configuration = configuration;
        }

        public async Task SendMessagesAsync(FileMessage[] messages)
        {
            var queueClient = QueueClient.Create(_configuration.QueueName);
            await queueClient.SendBatchAsync(messages.Select(x => new BrokeredMessage(x)));
        }
    }
}
