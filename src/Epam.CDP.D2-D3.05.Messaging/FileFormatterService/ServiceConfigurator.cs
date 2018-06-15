using Common;
using System;
using System.IO;
using Topshelf;
using Topshelf.StartParameters;

namespace FileFormatterService
{
    internal class ServiceConfigurator : IServiceConfigurator
    {
        private readonly IFileFormatterService _fileFormatterService;

        public ServiceConfigurator(IFileFormatterService fileFormatterService)
        {
            _fileFormatterService = fileFormatterService;
        }

        public void ConfigureAndRun()
        {
                HostFactory.Run(
                    x =>
                    {
                        x.EnableStartParameters();
                        x.Service<IFileFormatterService>(conf =>
                        {
                            conf.ConstructUsing(() => _fileFormatterService);
                            conf.WhenStarted((s, hostControl) => s.Start(hostControl));
                            conf.WhenStopped(s => s.Stop());
                            conf.WhenPaused(s => s.Stop());
                            conf.WhenContinued((s, hostControl) => s.Start(hostControl));
                        });

                        x.WithStartParameter("path", a =>
                        {
                            var paths = a.Split(';');
                            _fileFormatterService.MonitoringPaths.AddRange(paths);
                        });

                        x.WithStartParameter("timeout", a =>
                        {
                            if (string.IsNullOrEmpty(a))
                                return;

                            if (!int.TryParse(a, out int timeout))
                                throw new InvalidOperationException("'timeout' parameter should be a number.");

                            _fileFormatterService.NewPageTimeOut = timeout;
                        });

                        x.WithStartParameter("filetype", a =>
                        {
                            if (string.IsNullOrEmpty(a))
                                return;

                            if (!Enum.TryParse(a, true, out FileType fileType))
                                throw new InvalidOperationException("'filetype' parameter is wrong.");

                            _fileFormatterService.FileType = fileType;
                        });

                        x.WithStartParameter("damaged", a =>
                        {
                            if (!Directory.Exists(a))
                                throw new InvalidOperationException("'damaged' parameter path doesn't exist.");

                            _fileFormatterService.DamagedPath = a;
                        });

                        x.WithStartParameter("attempt", a =>
                        {
                            if (string.IsNullOrEmpty(a))
                                return;

                            if (!int.TryParse(a, out int attempt))
                                throw new InvalidOperationException("'attempt' parameter should be a number.");

                            _fileFormatterService.AttemptCount = attempt;
                        });

                        x.WithStartParameter("maxmsgsize", a =>
                        {
                            if (string.IsNullOrEmpty(a))
                                return;

                            if (!int.TryParse(a, out int maxMsgSize))
                                throw new InvalidOperationException("'attempt' parameter should be a number.");

                            _fileFormatterService.MaxMessagesSize = maxMsgSize;
                        });

                        x.WithStartParameter("nodename", a =>
                        {
                            if (string.IsNullOrEmpty(a))
                                return;

                            _fileFormatterService.NodeName = a;
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
