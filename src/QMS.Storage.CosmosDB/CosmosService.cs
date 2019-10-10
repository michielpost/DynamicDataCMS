using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using QMS.Models;
using QMS.Storage.CosmosDB.Models;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly CmsConfiguration cmsConfiguration;

        public CosmosService(IOptions<CosmosConfig> cosmosConfig, IOptions<CmsConfiguration> cmsConfiguration)
        {
            this.cosmosConfig = cosmosConfig.Value;
            this.cmsConfiguration = cmsConfiguration.Value;
        }
        internal async Task<(IReadOnlyList<CosmosCmsItem> results, int total)> List(string cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0, string? searchQuery = null)
        {
            Container container = GetContainer();
            var totalItems = await container.GetItemLinqQueryable<CosmosCmsItem>().Where(x => x.CmsType == cmsType).CountAsync().ConfigureAwait(false);
            int totalResults = totalItems.Resource;

            string whereClause = string.Empty;
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var typeInfo = cmsConfiguration.Entities.Where(x => x.Key == cmsType).FirstOrDefault();

                StringBuilder sb = new StringBuilder();
                sb.Append("AND (");
                foreach (var prop in typeInfo.ListViewProperties)
                    sb.Append($"CONTAINS(c.{prop.Key}, '{searchQuery}') OR ");
                sb.Remove(sb.Length - 3, 3);
                sb.Append(") ");

                whereClause = sb.ToString();

                //Only support one page of results for now
                //TODO: Create count query with Where clause
                totalResults = pageSize;
            }

            QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.cmstype = @cmstype {whereClause}OFFSET {pageSize*pageIndex} LIMIT {pageSize}").WithParameter("@cmstype", cmsType);

            if (sortField != null)
            {
                sortOrder = sortOrder ?? "Asc";
                queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.cmstype = @cmstype {whereClause}ORDER BY c.{sortField} {sortOrder.ToUpperInvariant()} OFFSET {pageSize * pageIndex} LIMIT {pageSize}")
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

            return (results, totalResults);
        }

        internal async Task Write(CosmosCmsDataItem item, string cmsType, Guid id, string? lang)
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

        public async Task Delete(string cmsType, Guid id, string? lang)
        {
            Container container = GetContainer();

            await container.DeleteItemAsync<CosmosCmsItem>(id.ToString(), new PartitionKey(cmsType)).ConfigureAwait(false);
        }

        internal async Task<CosmosCmsDataItem?> Read(string partitionKey, Guid documentId, string? lang)
        {
            var cmsItem = await ReadCmsItem(partitionKey, documentId).ConfigureAwait(false);

            CosmosCmsDataItem? data = cmsItem;

            if (lang != null)
                data = cmsItem?.Translations.FirstOrDefault(x => x.Key == lang).Value;

            return data;
        }


        internal async Task<CosmosCmsItem?> ReadCmsItem(string partitionKey, Guid documentId)
        {
            Container container = GetContainer();

            try
            {
                //TODO: Why does it throw a 404 when no document is found? Should not throw
                var response = await container.ReadItemAsync<CosmosCmsItem>(documentId.ToString(), new PartitionKey(partitionKey)).ConfigureAwait(false);

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
