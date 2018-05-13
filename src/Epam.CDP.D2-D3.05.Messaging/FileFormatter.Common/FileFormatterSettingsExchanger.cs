using System;
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

        public void SubscribeToSettingsSender(Func<FileFormatterSettings> settingsCollector, IServiceBusConfiguration configuration)
        {
            _ctsSender = new CancellationTokenSource();
            var queueController = new AzureMessagesTopicController(configuration);
            if (!queueController.QueueExist().GetAwaiter().GetResult())
                throw new InvalidOperationException($"Wrong configuration '{configuration.ConfigurationName}' or queue doesn't exist.");

            Task.Run(async () =>
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
                                NodeName = settings.NodeName
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

        public void SubscribeToNewSettingsReceiver(Action<FileFormatterSettings> onReceiveSettings, IServiceBusConfiguration configuration)
        {
            _receiverQueueController = new AzureMessagesTopicController(configuration);
            if (!_receiverQueueController.QueueExist().GetAwaiter().GetResult())
                throw new InvalidOperationException($"Wrong configuration '{configuration.ConfigurationName}' or queue doesn't exist.");

            Task.Run(() => _receiverQueueController.SubscribeToQueueAsync<SettingsMessageItem>(x =>
            {
                onReceiveSettings(new FileFormatterSettings
                {
                    NewPageTimeOut = x.NewPageTimeout,
                    ServiceStatus = (ServiceStatus)Enum.Parse(typeof(ServiceStatus), x.ServiceStatus),
                    NodeName = x.NodeName
                });

                return Task.FromResult(0);
            }));
        }

        public void UnSubscribeFromNewSettingsReceiver()
        {
            _receiverQueueController.UnSubscribeFromQueueAsync().Wait();
            _receiverQueueController = null;
        }
    }
}
