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


        private async Task<IEnumerable<SchemaLocation>> GetSchemas()
        {
            var result = new List<SchemaLocation>();

            var httpClient = clientFactory.CreateClient();

            if (cmsConfig == null || !cmsConfig.IsInitialized)
            {
                var config = await httpClient.GetStringAsync(cmsConfigLocation.Uri).ConfigureAwait(false);
                //TODO: Check if response is OK
                cmsConfig = JsonConvert.DeserializeObject<CmsConfiguration>(config);
            }

            //TODO: Cache schemas
            foreach (var location in cmsConfig.Entities.Where(x => x.Schema == null))
            {
                var getJson = await httpClient.GetStringAsync(location.Uri).ConfigureAwait(false);
                //JsonSchema schema = await JsonSchema.FromJsonAsync(getJson).ConfigureAwait(false);
                location.Schema = getJson;
            }

            return cmsConfig.Entities;
        }

        public Task InitializeSchemas()
        {
           return GetSchemas();
        }

        public SchemaLocation GetSchema(string cmsType)
        {
            return cmsConfig.Entities.Where(x => x.Key == cmsType).FirstOrDefault();
        }

        public CmsConfiguration GetCmsConfiguration()
        {
            return cmsConfig;
        }
    }
}
