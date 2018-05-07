using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace FileFormatterService
{
    public class AzureMessagesSender : IMessagesSender
    {
        private readonly ServiceBusConfiguration _configuration;

        public AzureMessagesSender(ServiceBusConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SendMessagesAsync(FileMessage[] messages)
        {
            var queueClient = QueueClient.CreateFromConnectionString(_configuration.ConnectionString, _configuration.QueueName);
            await queueClient.SendBatchAsync(messages.Select(x => new BrokeredMessage(x)));
        }
    }
}
