using System;
using System.IO;
using Common;
using Topshelf;
using Topshelf.StartParameters;
using System.Configuration;

namespace FileFormatterService
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

                    x.Service<FileFormatterService>(conf =>
                    {
                        conf.ConstructUsing(() => new FileFormatterService(new FileBuilderFactory(), new AzureMessagesSender(sbConf)));
                        conf.WhenStarted(s => s.Start());
                        conf.WhenStopped(s => s.Stop());
                    });

                    x.WithStartParameter("path", a =>
                    {
                        var paths = a.Split(';');
                        FileFormatterService.MonitoringPaths.AddRange(paths);
                    });

                    x.WithStartParameter("timeout", a =>
                    {
                        if (string.IsNullOrEmpty(a))
                            return;

                        if (!int.TryParse(a, out int timeout))
                            throw new InvalidOperationException("'timeout' parameter should be a number.");

                        FileFormatterService.NewPageTimeOut = timeout;
                    });

                    x.WithStartParameter("filetype", a =>
                    {
                        if (string.IsNullOrEmpty(a))
                            return;

                        if (!Enum.TryParse(a, true, out FileType fileType))
                            throw new InvalidOperationException("'filetype' parameter is wrong.");

                        FileFormatterService.FileType = fileType;
                    });

                    x.WithStartParameter("output", a =>
                    {
                        if (!Directory.Exists(a))
                            throw new InvalidOperationException("'output' parameter path doesn't exist.");

                        FileFormatterService.OutputPath = a;
                    });

                    x.WithStartParameter("damaged", a =>
                    {
                        if (!Directory.Exists(a))
                            throw new InvalidOperationException("'damaged' parameter path doesn't exist.");

                        FileFormatterService.DamagedPath = a;
                    });

                    x.WithStartParameter("attempt", a =>
                    {
                        if (string.IsNullOrEmpty(a))
                            return;

                        if (!int.TryParse(a, out int attempt))
                            throw new InvalidOperationException("'attempt' parameter should be a number.");

                        FileFormatterService.AttemptCount = attempt;
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
