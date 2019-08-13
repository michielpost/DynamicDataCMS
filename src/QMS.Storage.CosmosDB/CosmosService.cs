using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using QMS.Models;
using QMS.Storage.CosmosDB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QMS.Storage.CosmosDB
{
    public class CosmosService
    {
        private readonly CosmosConfig cosmosConfig;

        public CosmosService(IOptions<CosmosConfig> cosmosConfig)
        {
            this.cosmosConfig = cosmosConfig.Value;
        }
        public async Task<List<CmsItem>> List(string partitionKey)
        {
            Container container = GetContainer();

            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.cmstype = @cmstype").WithParameter("@cmstype", partitionKey);
            FeedIterator<CmsItem> queryResultSetIterator = container.GetItemQueryIterator<CmsItem>(queryDefinition);

            List<CmsItem> results = new List<CmsItem>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<CmsItem> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (CmsItem item in currentResultSet)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public async Task Save(string partitionKey, CmsItem document)
        {
            Container container = GetContainer();

            document.CmsType = partitionKey;
            await container.UpsertItemAsync(document, new PartitionKey(partitionKey));
        }

        public async Task<CmsItem> Load(string partitionKey, string documentId)
        {
            Container container = GetContainer();
            var response = await container.ReadItemAsync<CmsItem>(documentId, new PartitionKey(partitionKey));

            return response.Resource;

        }

        public async Task<Container> InitializeContainer()
        {
            CosmosClient client = new CosmosClient(cosmosConfig.Endpoint, cosmosConfig.Key);
            Database database = await client.CreateDatabaseIfNotExistsAsync(cosmosConfig.DatabaseId);
            Container container = await database.CreateContainerIfNotExistsAsync(
                cosmosConfig.ContainerId,
                "/cmstype",
                400);

            return container;
        }

        private Container GetContainer()
        {
            CosmosClient client = new CosmosClient(cosmosConfig.Endpoint, cosmosConfig.Key);
            Database database = client.GetDatabase(cosmosConfig.DatabaseId);
            Container container = database.GetContainer(cosmosConfig.ContainerId);

            return container;
        }
    }
}
