using System;
using System.Configuration;
using System.IO;
using MessagingApi.AzureServiceBus;
using Topshelf;
using Topshelf.StartParameters;
using C = FileFormatter.Common.Constants.ConfigurationConsts;

namespace FileFormatterCentralService
{
    class Program
    {
        static void Main()
        {
            var fileQueueConf = new ServiceBusConfiguration
            {
                ConfigurationName = C.FileQueueConfigName,
                ConnectionString = ConfigurationManager.AppSettings[C.ConnectionString],
                QueueName = ConfigurationManager.AppSettings[C.FileQueueName],
            };
            var statusQueueConf = new ServiceBusConfiguration
            {
                ConfigurationName = C.StatusQueueTopicName,
                ConnectionString = ConfigurationManager.AppSettings[C.ConnectionString],
                TopicName = ConfigurationManager.AppSettings[C.StatusTopicName],
                SubscriptionName = ConfigurationManager.AppSettings[C.StatusSubscriptionName],
            };
            var controlQueueConf = new ServiceBusConfiguration
            {
                ConfigurationName = C.ControlQueueName,
                ConnectionString = ConfigurationManager.AppSettings[C.ConnectionString],
                TopicName = ConfigurationManager.AppSettings[C.ControlTopicName],
            };

            HostFactory.Run(
                x =>
                {
                    x.EnableStartParameters();

                    x.Service<FileFormatterCentralService>(conf =>
                    {
                        conf.ConstructUsing(() => new FileFormatterCentralService(new FileAssembler1(), fileQueueConf, statusQueueConf, controlQueueConf));
                        conf.WhenStarted((s, hostControl) => s.Start(hostControl));
                        conf.WhenStopped(s => s.Stop());
                    });

                    x.WithStartParameter("output", a =>
                    {
                        if (!Directory.Exists(a))
                            throw new InvalidOperationException("'output' parameter path doesn't exist.");

                        FileFormatterCentralService.OutputPath = a;
                    });

                    x.WithStartParameter("systemfiles", a =>
                    {
                        if (!Directory.Exists(a))
                            throw new InvalidOperationException("'systemFiles' parameter path doesn't exist.");

                        FileFormatterCentralService.SystemFilesPath = a;
                    });

                    x.WithStartParameter("timeout", a =>
                    {
                        if (string.IsNullOrEmpty(a))
                            return;

                        if (!int.TryParse(a, out int timeout))
                            throw new InvalidOperationException("'timeout' parameter should be a number.");

                        FileFormatterCentralService.DefaultNewPageTimeout = timeout;
                    });

                    x.SetServiceName("FileFormatterCentralService");
                    x.SetDisplayName("File Formatter Central Service");
                    x.SetDescription("Centralized management service, which controls file formatter services.");
                    x.StartAutomaticallyDelayed();
                    x.RunAsLocalService();
                    x.EnableServiceRecovery(r =>
                    {
                        r.OnCrashOnly();
                        r.RestartService(1);
                        r.RestartService(1);
                        r.RestartService(1);
                        r.SetResetPeriod(1);
                    });
                });
        }
    }
}
