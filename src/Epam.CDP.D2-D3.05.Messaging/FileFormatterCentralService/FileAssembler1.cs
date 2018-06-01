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

        public void StartAssembling(string savePath, IMessagesController messageController)
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
                            var orderedSequence = assembledSequence.Messages
                                .OrderBy(y => y.Id)
                                .ToList();
                            using (var fs = new FileStream(Path.Combine(savePath, assembledSequence.FileName), FileMode.Create))
                            {
                                var offset = 0;
                                foreach (var item in orderedSequence)
                                {
                                    await fs.WriteAsync(item.Body, offset, item.Body.Length);
                                }

                                var bodyChecksum = Helper.ComputeChecksum(fs);
                                if (!string.Equals(assembledSequence.FileChecksum, bodyChecksum, StringComparison.Ordinal))
                                    continue;

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

        public void StopAssembling()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
}
