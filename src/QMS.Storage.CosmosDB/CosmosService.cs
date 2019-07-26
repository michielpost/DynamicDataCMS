using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace QMS.Storage.CosmosDB
{
    public class CosmosService
    {

        public async Task Save(dynamic document)
        {
            CosmosClient client = new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            Database database = await client.CreateDatabaseIfNotExistsAsync("qmsdb");
            Container container = await database.CreateContainerIfNotExistsAsync(
                "qms-container",
                "/cmstype",
                400);

            //set partition key
            document.cmstype = "pages";
            await container.UpsertItemAsync(document);
        }

        public async Task<JObject> Load(string documentId)
        {
            CosmosClient client = new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            Database database = await client.CreateDatabaseIfNotExistsAsync("qmsdb");
            Container container = await database.CreateContainerIfNotExistsAsync(
                "qms-container",
                "/cmstype",
                400);

            var response = await container.ReadItemAsync<JObject>(documentId, new PartitionKey("pages"));

            return response.Resource;

        }
    }
}
