using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IQueryable._01.E3SClient
{
	public class FtsRequestGenerator
	{
		private readonly UriTemplate _ftsSearchTemplate = new UriTemplate(@"data/searchFts?metaType={metaType}&query={query}&fields={fields}");
		private readonly Uri _baseAddress;

		public FtsRequestGenerator(string baseAddres) : this(new Uri(baseAddres))
		{
		}

		public FtsRequestGenerator(Uri baseAddress)
		{
			_baseAddress = baseAddress;
		}

		public Uri GenerateRequestUrl<T>(string query = "*", int start = 0, int limit = 10)
		{
			return GenerateRequestUrl(typeof(T), query, start, limit);
		}

		public Uri GenerateRequestUrl(Type type, string query = "*", int start = 0, int limit = 10)
		{
			string metaTypeName = GetMetaTypeName(type);

			var ftsQueryRequest = new FtsQueryRequest
			{
				Statements = new List<Statement>
				{
					new Statement {
						Query = query
					}
				},
				Start = start,
				Limit = limit
			};

			var ftsQueryRequestString = JsonConvert.SerializeObject(ftsQueryRequest);

			var uri = _ftsSearchTemplate.BindByName(_baseAddress,
				new Dictionary<string, string>
				{
					{ "metaType", metaTypeName },
					{ "query", ftsQueryRequestString }
				});

			return uri;
		}

		private string GetMetaTypeName(Type type)
		{
			var attributes = type.GetCustomAttributes(typeof(E3SMetaTypeAttribute), false);

			if (attributes.Length == 0)
				throw new Exception($"Entity {type.FullName} do not have attribute E3SMetaType");

			return ((E3SMetaTypeAttribute)attributes[0]).Name;
		}
	}
}
