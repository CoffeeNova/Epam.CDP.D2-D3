using System;
using System.Configuration;
using FileFormatter.Common;
using FileFormatter.Common.ServiceBusConfiguration;
using MessagingApi.AzureServiceBus;

namespace FileFormatterService.ServiceBusConfiguration
{
    public class ServiceBusConfigurationFactory : IServiceBusConfigurationFactory
    {
        public IServiceBusConfiguration CreateByType(SbConfigType configType)
        {
            switch (configType)
            {
                case SbConfigType.FileQueue:
                    return new MessagingApi.AzureServiceBus.ServiceBusConfiguration
                    {
                        ConfigurationName = Constants.ConfigurationConsts.FileQueueConfigName,
                        ConnectionString = ConfigurationManager.AppSettings[Constants.ConfigurationConsts.ConnectionString],
                        QueueName = ConfigurationManager.AppSettings[Constants.ConfigurationConsts.FileQueueName]
                    };
                case SbConfigType.StatusQueue:
                    return new MessagingApi.AzureServiceBus.ServiceBusConfiguration
                    {
                        ConfigurationName = Constants.ConfigurationConsts.StatusQueueTopicName,
                        ConnectionString = ConfigurationManager.AppSettings[Constants.ConfigurationConsts.ConnectionString],
                        TopicName = ConfigurationManager.AppSettings[Constants.ConfigurationConsts.StatusTopicName]
                    };
                case SbConfigType.ControlQueue:
                    return new MessagingApi.AzureServiceBus.ServiceBusConfiguration
                    {
                        ConfigurationName = Constants.ConfigurationConsts.ControlQueueName,
                        ConnectionString = ConfigurationManager.AppSettings[Constants.ConfigurationConsts.ConnectionString],
                        TopicName = ConfigurationManager.AppSettings[Constants.ConfigurationConsts.ControlTopicName]
                    };
                default:
                    throw new InvalidOperationException($"There is no ServiceBusConfiguration implementation for this {nameof(SbConfigType)}: {configType}");
            }
        }
    }
}
