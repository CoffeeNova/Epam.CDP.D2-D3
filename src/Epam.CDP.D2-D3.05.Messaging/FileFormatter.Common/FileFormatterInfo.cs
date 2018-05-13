using System;
using Newtonsoft.Json;

namespace FileFormatter.Common
{
    public class FileFormatterSettings
    {
        [JsonProperty("nodeName", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string NodeName { get; set; }
        [JsonProperty("serviceStatus", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ServiceStatus? ServiceStatus { get; set; }
        [JsonProperty("newPageTimeOut", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? NewPageTimeOut { get; set; }
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? Date { get; set; }
    }
}
