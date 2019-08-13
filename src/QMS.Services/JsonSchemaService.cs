using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NJsonSchema;
using QMS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace QMS.Services
{
    public class JsonSchemaService
    {
        private static CmsConfiguration cmsConfig;
        private readonly CmsConfigLocation cmsConfigLocation;
        private readonly IHttpClientFactory clientFactory;

        public JsonSchemaService(IOptions<CmsConfigLocation> schemaConfig, IHttpClientFactory clientFactory)
        {
            this.cmsConfigLocation = schemaConfig.Value;
            this.clientFactory = clientFactory;
        }


        public async Task<List<SchemaLocation>> GetSchemas()
        {
            var result = new List<SchemaLocation>();

            var httpClient = clientFactory.CreateClient();

            if (cmsConfig == null || !cmsConfig.IsInitialized)
            {
                var config = await httpClient.GetStringAsync(cmsConfigLocation.Uri);
                //TODO: Check if response is OK
                cmsConfig = JsonConvert.DeserializeObject<CmsConfiguration>(config);
            }

            //TODO: Cache schemas
            foreach (var location in cmsConfig.Entities.Where(x => x.Schema == null))
            {
                var getJson = await httpClient.GetStringAsync(location.Uri);
                JsonSchema schema = await JsonSchema.FromJsonAsync(getJson);
                location.Schema = getJson;
            }

            return cmsConfig.Entities;
        }

        public async Task<SchemaLocation> GetSchema(string cmsType)
        {
            var all = await GetSchemas();
            return all.Where(x => x.Key == cmsType).FirstOrDefault();
        }

        public CmsConfiguration GetCmsConfiguration()
        {
            return cmsConfig;
        }
    }
}
