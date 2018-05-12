using System;
using System.IO;
using System.Linq;
using Common;
using Newtonsoft.Json;

namespace MessagingApi
{
    public class MessageItem
    {
        [JsonProperty("id")]
        public int Id { get; private set; }
        [JsonProperty("body")]
        public byte[] Body { get; private set; }
        [JsonProperty("sequenceId")]
        public string SequenceId { get; private set; }
        [JsonProperty("sequenceCount")]
        public int SequenceCount { get; private set; }
        [JsonProperty("fileName")]
        public string FileName { get; private set; }
        [JsonProperty("fileChecksum")]
        public string FileChecksum { get; private set; }
        [JsonProperty("fileSize")]
        public long FileSize { get; private set; }

        public static MessageItem[] BuildFileMessageSequence(MemoryStream stream, string fileName, int maxBodySize)
        {
            var arr = stream.ToArray();
            var splittedByLength = arr.DivideByLength(maxBodySize).ToList();
            var guid = Guid.NewGuid().ToString();
            var checksum = Helper.ComputeChecksum(stream);

            return splittedByLength.Select((x, i) =>
            {
                var body = x.ToArray();
                return new MessageItem
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
    }
}
