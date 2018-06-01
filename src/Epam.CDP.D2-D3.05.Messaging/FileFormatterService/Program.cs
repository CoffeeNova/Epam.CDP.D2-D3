using System;
using System.IO;
using Common;
using Topshelf;
using Topshelf.StartParameters;
using System.Configuration;
using MessagingApi.AzureServiceBus;
using C = FileFormatter.Common.Constants.ConfigurationConsts;

namespace FileFormatterService
{
    class Program
    {
        static void Main()
        {
            var fileQueueConf = new ServiceBusConfiguration
            {
                ConfigurationName = C.FileQueueConfigName,
                ConnectionString = ConfigurationManager.AppSettings[C.ConnectionString],
                QueueName = ConfigurationManager.AppSettings[C.FileQueueName]
            };
            var statusQueueConf = new ServiceBusConfiguration
            {
                ConfigurationName = C.StatusQueueTopicName,
                ConnectionString = ConfigurationManager.AppSettings[C.ConnectionString],
                TopicName = ConfigurationManager.AppSettings[C.StatusTopicName]
            };
            var controlQueueConf = new ServiceBusConfiguration
            {
                ConfigurationName = C.ControlQueueName,
                ConnectionString = ConfigurationManager.AppSettings[C.ConnectionString],
                TopicName = ConfigurationManager.AppSettings[C.ControlTopicName]
            };

            HostFactory.Run(
                x =>
                {
                    x.EnableStartParameters();
                    x.Service<FileFormatterService>(conf =>
                    {
                        conf.ConstructUsing(() => new FileFormatterService(new FileBuilderFactory(), fileQueueConf, statusQueueConf, controlQueueConf));
                        conf.WhenStarted((s, hostControl) => s.Start(hostControl));
                        conf.WhenStopped(s => s.Stop());
                        conf.WhenPaused(s => s.Stop());
                        conf.WhenContinued((s, hostControl) => s.Start(hostControl));
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

                    x.WithStartParameter("maxmsgsize", a =>
                    {
                        if (string.IsNullOrEmpty(a))
                            return;

                        if (!int.TryParse(a, out int maxMsgSize))
                            throw new InvalidOperationException("'attempt' parameter should be a number.");

                        FileFormatterService.MaxMessagesSize = maxMsgSize;
                    });

                    x.WithStartParameter("nodename", a =>
                    {
                        if (string.IsNullOrEmpty(a))
                            return;

                        FileFormatterService.NodeName = a;
                    });

                    x.SetServiceName(nameof(FileFormatterService));
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
