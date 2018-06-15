using MessagingApi.AzureServiceBus;

namespace FileFormatter.Common.ServiceBusConfiguration
{
    public interface IServiceBusConfigurationFactory
    {
        IServiceBusConfiguration CreateByType(SbConfigType configType);
    }
}
