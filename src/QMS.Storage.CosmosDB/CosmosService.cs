using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using QMS.Models;
using QMS.Storage.CosmosDB.Models;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QMS.Storage.CosmosDB
{
    public class CosmosService : IReadCmsItem, IWriteCmsItem
    {
        private readonly CosmosConfig cosmosConfig;

        public CosmosService(IOptions<CosmosConfig> cosmosConfig)
        {
            this.cosmosConfig = cosmosConfig.Value;
        }
        public async Task<IReadOnlyList<CmsItem>> List(string cmsType)
        {
            Container container = GetContainer();

            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.cmstype = @cmstype").WithParameter("@cmstype", cmsType);
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

        public async Task Write(CmsItem item, string cmsType, string id, string lang)
        {
            Container container = GetContainer();

            item.CmsType = cmsType;
            await container.UpsertItemAsync(item, new PartitionKey(cmsType));
        }

        public async Task<CmsItem> Read(string partitionKey, string documentId)
        {
            Container container = GetContainer();

            try
            {
                //TODO: Why does it throw a 404 when no document is found? Should not throw
                var response = await container.ReadItemAsync<CmsItem>(documentId, new PartitionKey(partitionKey));

                return response.Resource;
            }
            catch { }

            return new CmsItem
            {
                Id = documentId,
                CmsType = partitionKey
            };
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
