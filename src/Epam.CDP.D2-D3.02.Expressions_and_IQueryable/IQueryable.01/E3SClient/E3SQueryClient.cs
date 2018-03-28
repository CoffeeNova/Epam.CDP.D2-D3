using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace IQueryable._01.E3SClient
{	
	public class E3SQueryClient
	{
		private readonly string _userName;
		private readonly string _password;
		private readonly Uri _baseAddress = new Uri("https://e3s.epam.com/eco/rest/e3s-eco-scripting-impl/0.1.0");


		public E3SQueryClient(string user, string password)
		{
			_userName = user;
			_password = password;
		}

		public IEnumerable<T> SearchFts<T>(string query, int start = 0, int limit = 0) where T : E3SEntity
		{
			HttpClient client = CreateClient();
			var requestGenerator = new FtsRequestGenerator(_baseAddress);

			Uri request = requestGenerator.GenerateRequestUrl<T>(query, start, limit);

			var resultString = client.GetStringAsync(request).Result;

			return JsonConvert.DeserializeObject<FtsResponse<T>>(resultString).Items.Select(t => t.Data);
		}


		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public IEnumerable SearchFts(Type type, string query, int start = 0, int limit = 0)
		{
			HttpClient client = CreateClient();
			var requestGenerator = new FtsRequestGenerator(_baseAddress);

			Uri request = requestGenerator.GenerateRequestUrl(type, query, start, limit);

			var resultString = client.GetStringAsync(request).Result;
			var endType = typeof(FtsResponse<>).MakeGenericType(type);
			var result = JsonConvert.DeserializeObject(resultString, endType);

			var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;

			foreach (object item in (IEnumerable)endType.GetProperty("items").GetValue(result))
			{
			    list?.Add(item.GetType().GetProperty("data").GetValue(item));
			}

			return list;
		}

		private HttpClient CreateClient()
		{
			var client = new HttpClient(new HttpClientHandler
			{
				AllowAutoRedirect = true,
				PreAuthenticate = true
			});

			var encoding = new ASCIIEncoding();
			var authHeader = new AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(encoding.GetBytes($"{_userName}:{_password}")));
			client.DefaultRequestHeaders.Authorization = authHeader;

			return client;
		}
	}
}
