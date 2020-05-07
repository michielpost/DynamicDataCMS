using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DynamicDataCMS.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DynamicDataCMS.Core.Services
{
    public class JsonSchemaService
    {
        private readonly CmsConfiguration cmsConfiguration;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<JsonSchemaService> logger;

        public JsonSchemaService(IOptions<CmsConfiguration> cmsConfiguration, IHttpClientFactory clientFactory, ILoggerFactory loggerFactory)
        {
            this.cmsConfiguration = cmsConfiguration.Value;
            this.clientFactory = clientFactory;
            this.logger = loggerFactory.CreateLogger<JsonSchemaService>();
        }


        private async Task<IEnumerable<SchemaLocation>> GetSchemas()
        {
            var result = new List<SchemaLocation>();

            var httpClient = clientFactory.CreateClient();

            //TODO: Cache schemas
            foreach (var location in cmsConfiguration.Schemas.Where(x => x.Schema == null))
            {
                try
                {
                    if (location.Uri != null)
                    {
                        var getJson = await httpClient.GetStringAsync(location.Uri).ConfigureAwait(false);
                        //JsonSchema schema = await JsonSchema.FromJsonAsync(getJson).ConfigureAwait(false);
                        location.Schema = getJson;
                    }
                    else if(!string.IsNullOrEmpty(location.FileLocation))
                    {
                        string json = File.ReadAllText(location.FileLocation);
                        location.Schema = json;
                    }
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Unable to read schema");
                }
            }

            return cmsConfiguration.SchemasInitialized;
        }

        public Task InitializeSchemas()
        {
           return GetSchemas();
        }

        public SchemaLocation GetSchema(CmsSchemaType schemaType)
        {
            return cmsConfiguration.Schemas.Where(x => x.Key == schemaType.Value).FirstOrDefault();
        }

        public MenuItem GetCmsType(CmsType cmsType)
        {
            return cmsConfiguration.MenuItems.Where(x => x.Key == cmsType.Value).FirstOrDefault();
        }

        public CmsConfiguration GetCmsConfiguration()
        {
            return cmsConfiguration;
        }
    }
}
