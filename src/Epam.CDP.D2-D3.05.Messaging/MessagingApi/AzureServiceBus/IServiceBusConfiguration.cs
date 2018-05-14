namespace MessagingApi.AzureServiceBus
{
    public interface IServiceBusConfiguration
    {
        string ConfigurationName { get; set; }
        string ConnectionString { get; set; }
        string QueueName { get; set; }
        string TopicName { get; set; }
        string SubscriptionName { get; set; }
    }
}