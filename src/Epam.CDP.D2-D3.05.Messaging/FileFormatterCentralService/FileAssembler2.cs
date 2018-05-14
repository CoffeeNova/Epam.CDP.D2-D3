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

        public void StartAssebling(string savePath, IMessagesController messageController)
        {
            _messageController = messageController;
            _savePath = savePath;
            Task.Run(async () => { await _messageController.SubscribeToQueueAsync(MessageHandler()); });
        }

        public void StopAssebling()
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
                    var body = assembledSequence.CollectBody();
                    var bodyChecksum = Helper.ComputeChecksum(body);
                    if (!string.Equals(assembledSequence.FileChecksum, bodyChecksum, StringComparison.Ordinal))
                        continue;

                    using (var fs = File.Create(Path.Combine(_savePath, assembledSequence.FileName), body.Length, FileOptions.Asynchronous))
                    {
                        await fs.WriteAsync(body, 0, body.Length);
                        await fs.FlushAsync();
                    }
                }
                _messagesContainer.RemoveAll(x => x.IsExpired || x.IsAssembled);
            };
        }
    }
}
