using System;
using MessagingApi.AzureServiceBus;

namespace FileFormatter.Common
{
    public interface IFileFormatterSettingsExchanger
    {
        void SubscribeToSettingsSender(Func<FileFormatterSettings> settingsCollector, IServiceBusConfiguration configuration);
        void UnSubscribeFromSettingsSender();
        void SubscribeToNewSettingsReceiver(Action<FileFormatterSettings> onReceiveSettings, IServiceBusConfiguration configuration);
        void UnSubscribeFromNewSettingsReceiver();
    }
}
