using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MessagingApi;

namespace FileFormatterCentralService
{
    public class FileFormatterCentralService
    {
        private readonly IMessagesController _messageController;
        private ServiceState _state;
        private CancellationTokenSource _cts;

        public FileFormatterCentralService(IMessagesController messageController)
        {
            _messageController = messageController;
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
            OutputPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "Output"));
#endif
        }

        public void Start()
        {
            if (_state == ServiceState.Started)
                return;

            _messageController.CreateQueue();

            _cts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    var messages = await _messageController.RecieveMessagesAsync(0);
                    if (!messages.Any())
                        continue;

                    var fileRepresents = messages
                        .GroupBy(x => x.MessageFamilyId)
                        .Select(x => new
                        {
                            Name = x.First().FileName,
                            Checksum = x.First().FileChecksum,
                            Body = x.OrderBy(y => y.Id).SelectMany(y => y.Body).ToArray()
                        });

                    foreach (var fp in fileRepresents)
                    {
                        var bodyChecksum = Helper.ComputeChecksum(fp.Body);
                        if (!string.Equals(fp.Checksum, bodyChecksum, StringComparison.Ordinal))
                        {
                            //to log
                            continue;
                        }

                        try
                        {
                            using (var fs = File.Create(Path.Combine(OutputPath, fp.Name), fp.Body.Length, FileOptions.Asynchronous))
                            {
                                await fs.WriteAsync(fp.Body, 0, fp.Body.Length);
                                await fs.FlushAsync();
                            }
                        }
                        catch
                        {
                            //to log
                        }
                    }

                    Thread.Sleep(2000);
                }

            }, _cts.Token);

            _state = ServiceState.Started;
        }

        public void Stop()
        {
            if (_state == ServiceState.Stopped)
                return;

            _cts.Cancel();
            _cts.Dispose();
            _state = ServiceState.Stopped;
        }

        private enum ServiceState
        {
            Stopped,
            Started
        }

        public static string OutputPath { get; set; }
    }
}
