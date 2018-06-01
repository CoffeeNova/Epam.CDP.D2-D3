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
    }
}