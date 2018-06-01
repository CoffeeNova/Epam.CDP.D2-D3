using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using FileFormatter.Common;
using MessagingApi;

namespace FileFormatterCentralService
{
    internal class FileAssembler2 : IFileAssembler
    {
        private readonly IDictionary<string, FileMessageSequence> _messagesContainer;
        private IMessagesController _messageController;
        private string _savePath;

        public FileAssembler2()
        {
            _messagesContainer = new Dictionary<string, FileMessageSequence>();
        }

        public void StartAssembling(string savePath, IMessagesController messageController)
        {
            _messageController = messageController;
            _savePath = savePath;
            Task.Run(async () => { await _messageController.SubscribeToQueueAsync(MessageHandler()); });
        }

        public void StopAssembling()
        {
            _messageController.UnSubscribeFromQueueAsync().Wait();
            _messageController = null;
        }

        private Func<FileMessageItem, Task> MessageHandler()
        {
            return async message =>
            {
                if (_messagesContainer.TryGetValue(message.SequenceId, out var messages))
                    messages?.Messages.Add(message);
                else
                    _messagesContainer.Add(message.SequenceId, new FileMessageSequence(_messageController.MessageTtl, DateTime.Now, message));

                if (!_messagesContainer.Any())
                    return;

                foreach (var assembledSequence in _messagesContainer.Values.Where(x => x.IsAssembled))
                {
                    var orderedSequence = assembledSequence.Messages
                        .OrderBy(y => y.Id)
                        .ToList();
                    using (var fs = new FileStream(Path.Combine(_savePath, assembledSequence.FileName), FileMode.Create))
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
            };
        }
    }
}
