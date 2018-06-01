using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using MessagingApi;
using MessagingApi.AzureServiceBus;

namespace FileFormatter.Common
{
    public class FileFormatterSettingsExchanger : IFileFormatterSettingsExchanger
    {
        private CancellationTokenSource _ctsSender;

        private const int SendSettingsPeriod = 10000;

        public async Task SubscribeToSettingsSender(Func<FileFormatterSettings> settingsCollector, IServiceBusConfiguration configuration)
        {
            _ctsSender = new CancellationTokenSource();
            var queueController = new AzureMessagesTopicController(configuration);
            if (!queueController.QueueExist().GetAwaiter().GetResult())
                throw new InvalidOperationException($"Wrong configuration '{configuration.ConfigurationName}' or queue doesn't exist.");

            await Task.Run(async () =>
             {
                 while (!_ctsSender.Token.IsCancellationRequested)
                 {
                     var settings = settingsCollector();
                     if (settings != null)
                     {
                         var settingsMessages = new[]
                         {
                            new SettingsMessageItem
                            {
                                NewPageTimeout = settings.NewPageTimeOut,
                                ServiceStatus = settings.ServiceStatus.HasValue
                                    ? Enum.GetName(settings.ServiceStatus.GetType(), settings.ServiceStatus)
                                    : null,
                                NodeName = settings.NodeName,
                                Date = settings.Date
                            }
                         };

                         try
                         {
                             await queueController.SendMessagesAsync(settingsMessages);
                         }
                         catch
                         {
                             //to log
                         }
                     }

                     Thread.Sleep(SendSettingsPeriod);
                 }
             }, _ctsSender.Token);
        }

        public void UnSubscribeFromSettingsSender()
        {
            _ctsSender.Cancel();
            _ctsSender.Dispose();
        }

        private IMessagesController _receiverQueueController;

        public async Task SubscribeToSettingsReceiver(Func<FileFormatterSettings, Task> onReceiveSettings, IServiceBusConfiguration configuration)
        {
            _receiverQueueController = new AzureMessagesTopicController(configuration);
            if (!await _receiverQueueController.QueueExist())
                throw new InvalidOperationException($"Wrong configuration '{configuration.ConfigurationName}' or queue doesn't exist.");

            await Task.Run(async () => await _receiverQueueController.SubscribeToQueueAsync<SettingsMessageItem>(x => onReceiveSettings(new FileFormatterSettings
            {
                NewPageTimeOut = x.NewPageTimeout,
                ServiceStatus = x.ServiceStatus != null
                    ? (ServiceControllerStatus?)Enum.Parse(typeof(ServiceControllerStatus), x.ServiceStatus)
                    : null,
                NodeName = x.NodeName,
                Date = x.Date
            })));
        }

        public void UnSubscribeFromSettingsReceiver()
        {
            try
            {
                _receiverQueueController.UnSubscribeFromQueueAsync().Wait();
                _receiverQueueController = null;
            }
            catch
            {
                //log
            }
        }
    }
}
