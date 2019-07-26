using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace QMS.Storage.CosmosDB
{
    public class CosmosService
    {

        public async Task Save(dynamic document)
        {
            CosmosClient client = new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            Database database = await client.CreateDatabaseIfNotExistsAsync("qms");
            Container container = await database.CreateContainerIfNotExistsAsync(
                "qms-container",
                "/cms",
                400);

            //dynamic testItem = new { id = "MyTestItemId", partitionKeyPath = "MyTestPkValue", details = "it's working" };
            //ItemResponse<dynamic> response = await container.CreateItemAsync(testItem);
            document.partitionKeyPath = "MyTestPkValue";

            await container.CreateItemAsync(document);
        }
    }
}
