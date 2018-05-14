using System;
using Newtonsoft.Json;

namespace MessagingApi
{
    public abstract class MessageItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("sequenceId")]
        public string SequenceId { get; set; }
        [JsonProperty("sequenceCount")]
        public int SequenceCount { get; set; }
        [JsonProperty("date")]
        public DateTime? Date { get; set; }
    }
}
