using Newtonsoft.Json;

namespace IQueryable._01.E3SClient.Entities
{
    public class Skills
    {
        [JsonProperty("nativespeaker")]
        public string Nativespeaker { get; set; }

        [JsonProperty("expert")]
        public string Expert { get; set; }

        [JsonProperty("advanced")]
        public string Advanced { get; set; }

        [JsonProperty("intermediate")]
        public string Intermediate { get; set; }

        [JsonProperty("novice")]
        public string Novice { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("os")]
        public string Os { get; set; }

        [JsonProperty("db")]
        public string Db { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("industry")]
        public string Industry { get; set; }

        [JsonProperty("proglang")]
        public string Proglang { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("other")]
        public string Other { get; set; }

        [JsonProperty("primary")]
        public string Primary { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}
