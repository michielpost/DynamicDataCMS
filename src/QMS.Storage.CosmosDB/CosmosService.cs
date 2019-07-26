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
                "/name",
                400);

            await container.UpsertItemAsync(document);
        }
    }
}
