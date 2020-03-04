using Microsoft.Extensions.Options;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Storage.CosmosDB.Extensions;
using DynamicDataCms.Storage.CosmosDB.Models;
using DynamicDataCms.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCms.Storage.CosmosDB
{
    /// <summary>
    /// CosmosDB does not serialize with System.Text.Json, this wrapper converts the System.Text.json objects to newtonsoft compatible objects
    /// </summary>
    public class CosmosWrapperService : IReadCmsItem, IWriteCmsItem
    {
        internal readonly CosmosService _cosmosService;
        private readonly CosmosConfig cosmosConfig;

        public bool CanSort(string cmsType) => true;
        public bool HandlesType(string cmsType) => !cosmosConfig.ExcludedTypes.Contains(cmsType);


        public CosmosWrapperService(CosmosService cosmosService, IOptions<CosmosConfig> cosmosConfig)
        {
            _cosmosService = cosmosService;
            this.cosmosConfig = cosmosConfig.Value;
        }
        public async Task<(IReadOnlyList<CmsItem> results, int total)> List(string cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0, string? searchQuery = null)
        {
            var result = await _cosmosService.List(cmsType, sortField, sortOrder, pageSize, pageIndex, searchQuery).ConfigureAwait(false);
            return (result.results.Select(x => x.ToCmsItem()).ToList(), result.total);
        }

        public Task Write<T>(T item, string cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem
        {
            return _cosmosService.Write(item.ToCosmosCmsItem(), cmsType, id, lang);
        }

        public Task Delete(string cmsType, Guid id, string? lang, string? currentUser)
        {
            return _cosmosService.Delete(cmsType, id, lang);

        }

        public async Task<T?> Read<T>(string partitionKey, Guid documentId, string? lang) where T : CmsItem
        {
            var result = await _cosmosService.Read(partitionKey, documentId, lang).ConfigureAwait(false);
            return result?.ToCmsItem<T>();
        }
    }
}
