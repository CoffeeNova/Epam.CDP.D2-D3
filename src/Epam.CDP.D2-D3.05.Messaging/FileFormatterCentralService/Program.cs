using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        conf.WhenStarted(s => s.Start());
                        conf.WhenStopped(s => s.Stop());
                    });

                    x.WithStartParameter("output", a =>
                    {
                        if (!Directory.Exists(a))
                            throw new InvalidOperationException("'output' parameter path doesn't exist.");

                        FileFormatterCentralService.OutputPath = a;
                    });

                    x.SetServiceName("FileFormatterService");
                    x.SetDisplayName("File Formatter Service");
                    x.SetDescription("Service which glues images together in single file of the specified type");
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
