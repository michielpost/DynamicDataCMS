using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using QMS.Models;
using QMS.Storage.CosmosDB.Models;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Storage.CosmosDB
{
    /// <summary>
    /// Implements read and write interface for CmsItems for CosmosDB storage
    /// </summary>
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
            FeedIterator<CosmosDBCmsItem> queryResultSetIterator = container.GetItemQueryIterator<CosmosDBCmsItem>(queryDefinition);

            List<CosmosDBCmsItem> results = new List<CosmosDBCmsItem>();

            while (queryResultSetIterator.HasMoreResults)
            {
                try
                {
                    FeedResponse<CosmosDBCmsItem> currentResultSet = await queryResultSetIterator.ReadNextAsync().ConfigureAwait(false);
                    foreach (CosmosDBCmsItem item in currentResultSet)
                    {
                        results.Add(item);
                    }
                }
                catch (CosmosException)
                {
                    //TODO: Handle scenario if no documents are found, first time. Not initialized yet?
                    break;
                }
            }

            return results;
        }

        public async Task Write(CmsItem item, string cmsType, string id, string? lang)
        {
            Container container = GetContainer();

            item.CmsType = cmsType;


            //Read main item without language
            var cmsItem = await this.ReadCmsItem(cmsType, id).ConfigureAwait(false);
            if (cmsItem == null)
                cmsItem = new CosmosDBCmsItem();

            cmsItem.Id = id;
            cmsItem.CmsType = cmsType;

            if (lang == null)
                cmsItem.AdditionalProperties = item.AdditionalProperties;
            else
                cmsItem.Translations[lang] = item;


            await container.UpsertItemAsync(item, new PartitionKey(cmsType)).ConfigureAwait(false);
        }

        public async Task Delete(string cmsType, string id, string? lang)
        {
            Container container = GetContainer();

            await container.DeleteItemAsync<CosmosDBCmsItem>(id, new PartitionKey(cmsType)).ConfigureAwait(false);
        }

        public async Task<CmsItem?> Read(string partitionKey, string documentId, string? lang)
        {
            var cmsItem = await ReadCmsItem(partitionKey, documentId);

            CmsItem? data = cmsItem;

            if (lang != null)
                data = cmsItem?.Translations.FirstOrDefault(x => x.Key == lang).Value;

            return data;
        }


        internal async Task<CosmosDBCmsItem?> ReadCmsItem(string partitionKey, string documentId)
        {
            Container container = GetContainer();

            try
            {
                //TODO: Why does it throw a 404 when no document is found? Should not throw
                var response = await container.ReadItemAsync<CosmosDBCmsItem>(documentId, new PartitionKey(partitionKey)).ConfigureAwait(false);

                var cmsItem = response.Resource;

                return response.Resource;
            }
            catch { }

            //TODO: return null?
            return new CosmosDBCmsItem
            {
                Id = documentId,
                CmsType = partitionKey
            };
        }

        public async Task<Container> InitializeContainer()
        {
            CosmosClient client = new CosmosClient(cosmosConfig.Endpoint, cosmosConfig.Key);
            Database database = await client.CreateDatabaseIfNotExistsAsync(cosmosConfig.DatabaseId).ConfigureAwait(false);
            Container container = await database.CreateContainerIfNotExistsAsync(
                cosmosConfig.ContainerId,
                "/cmstype",
                400).ConfigureAwait(false);

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
