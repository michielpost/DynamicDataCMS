using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Options;
using QMS.Models;
using QMS.Storage.CosmosDB.Models;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace QMS.Storage.CosmosDB
{
    /// <summary>
    /// Implements read and write interface for CmsItems for CosmosDB storage
    /// </summary>
    public class CosmosService
    {
        private readonly CosmosConfig cosmosConfig;

        public CosmosService(IOptions<CosmosConfig> cosmosConfig)
        {
            this.cosmosConfig = cosmosConfig.Value;
        }
        internal async Task<IReadOnlyList<CosmosCmsItem>> List(string cmsType, string? sortField, string sortOrder = "Asc")
        {
            Container container = GetContainer();

            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.cmstype = @cmstype").WithParameter("@cmstype", cmsType);
            if (sortField != null)
            {
                queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.cmstype = @cmstype ORDER BY c.{sortField} {sortOrder.ToUpperInvariant()}")
                    .WithParameter("@cmstype", cmsType);
            }

            FeedIterator<CosmosCmsItem> queryResultSetIterator = container.GetItemQueryIterator<CosmosCmsItem>(queryDefinition);

            List<CosmosCmsItem> results = new List<CosmosCmsItem>();

            while (queryResultSetIterator.HasMoreResults)
            {
                try
                {
                    FeedResponse<CosmosCmsItem> currentResultSet = await queryResultSetIterator.ReadNextAsync().ConfigureAwait(false);
                    foreach (CosmosCmsItem item in currentResultSet)
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

        internal async Task Write(CosmosCmsDataItem item, string cmsType, string id, string? lang)
        {
            Container container = GetContainer();

            item.Id = id;
            item.CmsType = cmsType;


            //Read main item without language
            var cmsItem = await this.ReadCmsItem(cmsType, id).ConfigureAwait(false);
            if (cmsItem == null)
                cmsItem = new CosmosCmsItem();

            cmsItem.Id = id;
            cmsItem.CmsType = cmsType;

            if (lang == null)
                cmsItem.AdditionalProperties = item.AdditionalProperties;
            else
                cmsItem.Translations[lang] = item;


            await container.UpsertItemAsync(cmsItem, new PartitionKey(cmsType)).ConfigureAwait(false);
        }

        public async Task Delete(string cmsType, string id, string? lang)
        {
            Container container = GetContainer();

            await container.DeleteItemAsync<CosmosCmsItem>(id, new PartitionKey(cmsType)).ConfigureAwait(false);
        }

        internal async Task<CosmosCmsDataItem?> Read(string partitionKey, string documentId, string? lang)
        {
            var cmsItem = await ReadCmsItem(partitionKey, documentId);

            CosmosCmsDataItem? data = cmsItem;

            if (lang != null)
                data = cmsItem?.Translations.FirstOrDefault(x => x.Key == lang).Value;

            return data;
        }


        internal async Task<CosmosCmsItem?> ReadCmsItem(string partitionKey, string documentId)
        {
            Container container = GetContainer();

            try
            {
                //TODO: Why does it throw a 404 when no document is found? Should not throw
                var response = await container.ReadItemAsync<CosmosCmsItem>(documentId, new PartitionKey(partitionKey)).ConfigureAwait(false);

                var cmsItem = response.Resource;

                return response.Resource;
            }
            catch { }

            //TODO: return null?
            return new CosmosCmsItem
            {
                Id = documentId,
                CmsType = partitionKey
            };
        }

        public async Task<Container> InitializeContainer()
        {
            CosmosClient client = GetCosmosClient();
            Database database = await client.CreateDatabaseIfNotExistsAsync(cosmosConfig.DatabaseId).ConfigureAwait(false);
            Container container = await database.CreateContainerIfNotExistsAsync(
                cosmosConfig.ContainerId,
                "/cmstype",
                400).ConfigureAwait(false);

            return container;
        }

        internal CosmosClient GetCosmosClient()
        {
            return new CosmosClient(cosmosConfig.Endpoint, cosmosConfig.Key);
        }

        internal Container GetContainer()
        {
            CosmosClient client = GetCosmosClient();
            Database database = client.GetDatabase(cosmosConfig.DatabaseId);
            Container container = database.GetContainer(cosmosConfig.ContainerId);

            return container;
        }
    }
}
