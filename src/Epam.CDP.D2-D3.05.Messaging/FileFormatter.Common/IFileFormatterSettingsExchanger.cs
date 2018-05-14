using System;
using System.Threading.Tasks;
using MessagingApi.AzureServiceBus;

namespace FileFormatter.Common
{
    public interface IFileFormatterSettingsExchanger
    {
        Task SubscribeToSettingsSender(Func<FileFormatterSettings> settingsCollector, IServiceBusConfiguration configuration);
        void UnSubscribeFromSettingsSender();
        Task SubscribeToSettingsReceiver(Func<FileFormatterSettings, Task> onReceiveSettings, IServiceBusConfiguration configuration);
        void UnSubscribeFromSettingsReceiver();
    }
}
