namespace MessagingApi.AzureServiceBus
{
    public class ServiceBusConfiguration : IServiceBusConfiguration
    {
        public string ConfigurationName { get; set; }
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
    }
}
