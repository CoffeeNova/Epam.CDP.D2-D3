using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Common;

namespace MessagingApi
{
    public class MessageItem
    {
        public int Id { get; private set; }
        public byte[] Body { get; private set; }
        public int Length { get; private set; }
        public string MessageFamilyId { get; private set; }
        public string FileName { get; private set; }
        public string FileChecksum { get; private set; }

        public static MessageItem[] BuildFileMessageSequence(MemoryStream stream, string fileName, int maxMessagesSize)
        {
            var arr = stream.ToArray();
            var splittedByLength = arr.DivideByLength(maxMessagesSize);
            var guid = Guid.NewGuid().ToString();
            var checksum = Helper.ComputeChecksum(stream);

            return splittedByLength.Select((x, i) =>
            {
                var body = x.ToArray();
                return new MessageItem
                {
                    Id = i,
                    Body = body,
                    Length = body.Length,
                    MessageFamilyId = guid,
                    FileName = fileName,
                    FileChecksum = checksum
                };
            }).ToArray();
        }
    }
}
