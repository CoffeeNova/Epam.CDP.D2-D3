using System;
using System.Configuration;
using System.IO;
using MessagingApi.AzureServiceBus;
using Topshelf;
using Topshelf.StartParameters;

namespace FileFormatterCentralService
{
    class Program
    {
        static void Main()
        {
            var sbConf = new ServiceBusConfiguration
            {
                ConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"],
                QueueName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.QueueName"]
            };

            HostFactory.Run(
                x =>
                {
                    x.EnableStartParameters();

                    x.Service<FileFormatterCentralService>(conf =>
                    {
                        conf.ConstructUsing(() => new FileFormatterCentralService(new AzureMessagesController(sbConf)));
                        conf.WhenStarted((s, hostControl) => s.Start(hostControl));
                        conf.WhenStopped(s => s.Stop());
                    });

                    x.WithStartParameter("output", a =>
                    {
                        if (!Directory.Exists(a))
                            throw new InvalidOperationException("'output' parameter path doesn't exist.");

                        FileFormatterCentralService.OutputPath = a;
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
