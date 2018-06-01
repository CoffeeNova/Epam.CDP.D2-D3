using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;

namespace FileFormatter.Common
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

        public static FileMessageItem[] BuildFileMessageItems(Stream stream, string fileName, int maxBodySize)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var arr = stream.StreamToIEnumerable();
            var splittedByLength = arr.DivideByLength(maxBodySize).ToList();
            var guid = Guid.NewGuid().ToString();
            var checksum = Helper.ComputeChecksum(stream);

            return splittedByLength.Select((x, i) =>
            {
                var body = x.ToArray();
                return new FileMessageItem
                {
                    Id = i,
                    Body = body,
                    SequenceId = guid,
                    FileName = fileName,
                    FileChecksum = checksum,
                    FileSize = stream.Length,
                    SequenceCount = splittedByLength.Count
                };
            }).ToArray();
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
