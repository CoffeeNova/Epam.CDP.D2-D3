using Newtonsoft.Json;

namespace IQueryable._01.E3SClient.Entities
{
    public class WorkHistory
    {
        [JsonProperty("terms")]
        public string Terms { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("companyurl")]
        public string Companyurl { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("project")]
        public string Project { get; set; }

        [JsonProperty("participation")]
        public string Participation { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("database")]
        public string Database { get; set; }

        [JsonProperty("tools")]
        public string Tools { get; set; }

        [JsonProperty("technologies")]
        public string Technologies { get; set; }

        [JsonProperty("startdate")]
        public string Startdate { get; set; }

        [JsonProperty("enddate")]
        public string Enddate { get; set; }

        [JsonProperty("isepam")]
        public bool Isepam { get; set; }

        [JsonProperty("epamproject")]
        public string Epamproject { get; set; }
    }
}
