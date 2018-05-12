using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MessagingApi;
using Topshelf;

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
#if DEBUG
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            OutputPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", "Output"));
#endif
        }

        public bool Start(HostControl hostControl)
        {
            if (_state == ServiceState.Started)
                return true;

            if (!TryCreateQueue())
            {
                hostControl.Stop();
                return true;
            }

            StartMessagesMonitor();

            _state = ServiceState.Started;
            return true;
        }

        public void Stop()
        {
            if (_state == ServiceState.Stopped)
                return;

            StopMessagesMonitor();
            _state = ServiceState.Stopped;
        }

        private bool TryCreateQueue()
        {
            try
            {
                _messageController.CreateQueue().GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                //to log
                return false;
            }
        }

        private void StartMessagesMonitor()
        {
            _cts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        var message = await _messageController.RecieveMessageAsync();
                        if (message != null)
                        {
                            if (_messagesContainer.TryGetValue(message.SequenceId, out var messages))
                                messages?.Messages.Add(message);
                            else
                                _messagesContainer.Add(message.SequenceId, new MessageSequence(_messageController.MessageTtl, DateTime.Now, message));
                        }

                        if (!_messagesContainer.Any())
                            continue;

                        foreach (var assembledSequence in _messagesContainer.Values.Where(x => x.IsAssembled))
                        {
                            var body = assembledSequence.CollectBody();
                            var bodyChecksum = Helper.ComputeChecksum(body);
                            if (!string.Equals(assembledSequence.FileChecksum, bodyChecksum, StringComparison.Ordinal))
                                continue;

                            using (var fs = File.Create(Path.Combine(OutputPath, assembledSequence.FileName), body.Length, FileOptions.Asynchronous))
                            {
                                await fs.WriteAsync(body, 0, body.Length);
                                await fs.FlushAsync();
                            }
                        }
                        _messagesContainer.RemoveAll(x => x.IsExpired || x.IsAssembled);


                        Thread.Sleep(2000);
                    }
                    catch
                    {
                        //to log
                    }
                }

            }, _cts.Token);
        }

        private readonly IDictionary<string, MessageSequence> _messagesContainer = new Dictionary<string, MessageSequence>();
        private class MessageSequence
        {
            public MessageSequence(int sequenceTtl, DateTime createdDate, MessageItem message)
            {
                _sequenceTtl = sequenceTtl;
                _createdDate = createdDate;
                _sequenceCount = message.SequenceCount;
                Messages = new List<MessageItem> { message };
                FileName = message.FileName;
                FileChecksum = message.FileChecksum;
            }

            public byte[] CollectBody()
            {
                return Messages.OrderBy(y => y.Id).SelectMany(y => y.Body).ToArray();
            }

            private readonly int _sequenceTtl;
            private readonly DateTime _createdDate;
            private readonly int _sequenceCount;

            public ICollection<MessageItem> Messages { get; }
            public bool IsExpired => DateTime.Now.Subtract(_createdDate).TotalSeconds > _sequenceTtl;
            public bool IsAssembled => Messages.Count == _sequenceCount;

            public string FileName { get; }
            public string FileChecksum { get; }
        }

        private void StopMessagesMonitor()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        private enum ServiceState
        {
            Stopped,
            Started
        }

        public static string OutputPath { get; set; }
    }
}
