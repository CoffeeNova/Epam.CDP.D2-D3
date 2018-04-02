using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IQueryable._01.E3SClient
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FTSRequestGenerator
    {
        private readonly UriTemplate FTSSearchTemplate = new UriTemplate(@"data/searchFts?metaType={metaType}&query={query}&fields={fields}");
        private readonly Uri BaseAddress;

        public FTSRequestGenerator(string baseAddres) : this(new Uri(baseAddres))
        {
        }

        public FTSRequestGenerator(Uri baseAddress)
        {
            BaseAddress = baseAddress;
        }

        public Uri GenerateRequestUrl<T>(int start = 0, int limit = 10, params string[] query)
        {
            return GenerateRequestUrl(typeof(T), start, limit, query);
        }

        public Uri GenerateRequestUrl(Type type, int start = 0, int limit = 10, params string[] query)
        {
            if (!query.Any())
                query = new[] { "*" };

            string metaTypeName = GetMetaTypeName(type);

            var ftsQueryRequest = new FTSQueryRequest
            {
                Statements = query.Select(x => new Statement { Query = x }).ToList(),
                Start = start,
                Limit = limit
            };

            var ftsQueryRequestString = JsonConvert.SerializeObject(ftsQueryRequest);

            var uri = FTSSearchTemplate.BindByName(BaseAddress,
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
                throw new Exception(string.Format("Entity {0} do not have attribute E3SMetaType", type.FullName));

            return ((E3SMetaTypeAttribute)attributes[0]).Name;
        }

    }
}
