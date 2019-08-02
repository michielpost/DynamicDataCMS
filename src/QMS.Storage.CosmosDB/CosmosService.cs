using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QMS.Storage.CosmosDB
{
    public class CosmosService
    {

        public async Task<List<dynamic>> List(string partitionKey)
        {
            CosmosClient client = new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            Database database = await client.CreateDatabaseIfNotExistsAsync("qmsdb");
            Container container = await database.CreateContainerIfNotExistsAsync(
                "qms-container",
                "/cmstype",
                400);

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
            CosmosClient client = new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            Database database = await client.CreateDatabaseIfNotExistsAsync("qmsdb");
            Container container = await database.CreateContainerIfNotExistsAsync(
                "qms-container",
                "/cmstype",
                400);

            document.cmstype = partitionKey;
            await container.UpsertItemAsync(document, new PartitionKey(partitionKey));
        }

        public async Task<JObject> Load(string partitionKey, string documentId)
        {
            CosmosClient client = new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            Database database = await client.CreateDatabaseIfNotExistsAsync("qmsdb");
            Container container = await database.CreateContainerIfNotExistsAsync(
                "qms-container",
                "/cmstype",
                400);

            var response = await container.ReadItemAsync<JObject>(documentId, new PartitionKey(partitionKey));

            return response.Resource;

        }
    }
}
