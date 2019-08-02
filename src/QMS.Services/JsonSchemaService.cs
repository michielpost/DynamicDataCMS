using Microsoft.Extensions.Options;
using Newtonsoft.Json.Schema;
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
        private readonly JsonSchemaConfig schemaConfig;
        private readonly IHttpClientFactory clientFactory;

        public JsonSchemaService(IOptions<JsonSchemaConfig> schemaConfig, IHttpClientFactory clientFactory)
        {
            this.schemaConfig = schemaConfig.Value;
            this.clientFactory = clientFactory;
        }


        public async Task<List<SchemaLocation>> GetSchemas()
        {
            var result = new List<SchemaLocation>();

            var httpClient = clientFactory.CreateClient();

            //TODO: Cache schemas

            foreach (var location in schemaConfig.SchemaLocations.Where(x => x.Schema == null))
            {
                var getJson = await httpClient.GetStringAsync(location.Uri);
               // JsonSchema schema = JsonSchema.Parse(getJson);
                location.Schema = getJson;
            }

            return schemaConfig.SchemaLocations;
        }

        public async Task<SchemaLocation> GetSchema(string cmsType)
        {
            var all = await GetSchemas();
            return all.Where(x => x.Key == cmsType).FirstOrDefault();
        }
    }
}
