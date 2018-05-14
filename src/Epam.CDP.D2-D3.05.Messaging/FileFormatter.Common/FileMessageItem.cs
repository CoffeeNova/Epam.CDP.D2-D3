using System;
using System.IO;
using System.Linq;
using Common;
using MessagingApi;
using Newtonsoft.Json;

namespace FileFormatter.Common
{
    public class FileMessageItem : MessageItem
    {
        [JsonProperty("body")]
        public byte[] Body { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("fileChecksum")]
        public string FileChecksum { get; set; }
        [JsonProperty("fileSize")]
        public long FileSize { get; set; }

        public static FileMessageItem[] BuildFileMessageSequence(MemoryStream stream, string fileName, int maxBodySize)
        {
            var arr = stream.ToArray();
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
    }
}
