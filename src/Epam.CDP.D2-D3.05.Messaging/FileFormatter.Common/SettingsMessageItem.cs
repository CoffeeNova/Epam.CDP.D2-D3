using MessagingApi;
using Newtonsoft.Json;

namespace FileFormatter.Common
{
    public class SettingsMessageItem : MessageItem
    {
        [JsonProperty("serviceStatus")]
        public string ServiceStatus { get; set; }

        [JsonProperty("newPageTimeout")]
        public int? NewPageTimeout { get; set; }

        [JsonProperty("nodeName")]
        public string NodeName { get; set; }
    }
}
