using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using FileFormatter.Common;
using MessagingApi;

namespace FileFormatterCentralService
{
    internal class FileAssembler1 : IFileAssembler
    {
        private CancellationTokenSource _cts;
        private readonly IDictionary<string, FileMessageSequence> _messagesContainer;

        public FileAssembler1()
        {
            _messagesContainer = new Dictionary<string, FileMessageSequence>();
        }

        public void StartAssebling(string savePath, IMessagesController messageController)
        {
            _cts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        var message = await messageController.RecieveMessageAsync<FileMessageItem>();
                        if (message != null)
                        {
                            if (_messagesContainer.TryGetValue(message.SequenceId, out var messages))
                                messages?.Messages.Add(message);
                            else
                                _messagesContainer.Add(message.SequenceId, new FileMessageSequence(messageController.MessageTtl, DateTime.Now, message));
                        }

                        if (!_messagesContainer.Any())
                            continue;

                        foreach (var assembledSequence in _messagesContainer.Values.Where(x => x.IsAssembled))
                        {
                            var body = assembledSequence.CollectBody();
                            var bodyChecksum = Helper.ComputeChecksum(body);
                            if (!string.Equals(assembledSequence.FileChecksum, bodyChecksum, StringComparison.Ordinal))
                                continue;

                            using (var fs = File.Create(Path.Combine(savePath, assembledSequence.FileName), body.Length, FileOptions.Asynchronous))
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

        public void StopAssebling()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
}
