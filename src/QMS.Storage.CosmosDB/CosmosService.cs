using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
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
        public async Task<List<dynamic>> List(string partitionKey)
        {
            Container container = GetContainer();

            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.cmstype = @cmstype").WithParameter("@cmstype", partitionKey);
            FeedIterator<dynamic> queryResultSetIterator = container.GetItemQueryIterator<dynamic>(queryDefinition);

            List<dynamic> results = new List<dynamic>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<dynamic> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (dynamic item in currentResultSet)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public async Task Save(string partitionKey, dynamic document)
        {
            Container container = GetContainer();

            document.cmstype = partitionKey;
            await container.UpsertItemAsync(document, new PartitionKey(partitionKey));
        }

        public async Task<JObject> Load(string partitionKey, string documentId)
        {
            Container container = GetContainer();
            var response = await container.ReadItemAsync<JObject>(documentId, new PartitionKey(partitionKey));

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
