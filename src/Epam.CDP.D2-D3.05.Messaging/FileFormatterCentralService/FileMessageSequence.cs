using System;
using System.Collections.Generic;
using System.Linq;
using FileFormatter.Common;

namespace FileFormatterCentralService
{
    public class FileMessageSequence
    {
        public FileMessageSequence(int sequenceTtl, DateTime createdDate, FileMessageItem message)
        {
            _sequenceTtl = sequenceTtl;
            _createdDate = createdDate;
            _sequenceCount = message.SequenceCount;
            Messages = new List<FileMessageItem> { message };
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

        public ICollection<FileMessageItem> Messages { get; }
        public bool IsExpired => DateTime.Now.Subtract(_createdDate).TotalSeconds > _sequenceTtl;
        public bool IsAssembled => Messages.Count == _sequenceCount;

        public string FileName { get; }
        public string FileChecksum { get; }
    }
}
