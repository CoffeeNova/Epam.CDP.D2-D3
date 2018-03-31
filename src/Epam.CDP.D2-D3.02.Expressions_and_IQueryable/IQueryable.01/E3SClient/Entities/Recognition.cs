using Newtonsoft.Json;

namespace IQueryable._01.E3SClient.Entities
{
    public class Recognition
    {
        [JsonProperty("nomination")]
        public string Nomination { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("recognitiondate")]
        public string Recognitiondate { get; set; }

        [JsonProperty("points")]
        public string Points { get; set; }
    }
}
