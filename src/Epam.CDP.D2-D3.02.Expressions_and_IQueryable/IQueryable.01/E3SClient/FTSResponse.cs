using System.Collections.Generic;
using Newtonsoft.Json;

namespace IQueryable._01.E3SClient
{
	public class Item<T>
	{
	    [JsonProperty("data")]
        public T Data { get; set; }

	}

	public class FtsResponse<T> where T : class
	{
        [JsonProperty("total")]
		public int Total { get; set; }

	    [JsonProperty("items")]
        public List<Item<T>> Items { get; set; }
	}
}
