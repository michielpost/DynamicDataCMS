﻿using Microsoft.Extensions.Options;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.CosmosDB.Extensions;
using DynamicDataCMS.Storage.CosmosDB.Models;
using DynamicDataCMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Storage.CosmosDB
{
    /// <summary>
    /// CosmosDB does not serialize with System.Text.Json, this wrapper converts the System.Text.json objects to newtonsoft compatible objects
    /// </summary>
    public class CosmosWrapperService : IReadCmsItem, IWriteCmsItem
    {
        internal readonly CosmosService _cosmosService;
        private readonly CosmosConfig cosmosConfig;

        public bool CanSort(CmsType cmsType) => true;
        public bool HandlesType(CmsType cmsType) => !cosmosConfig.ExcludedTypes.Contains(cmsType);


        public CosmosWrapperService(CosmosService cosmosService, IOptions<CosmosConfig> cosmosConfig)
        {
            _cosmosService = cosmosService;
            this.cosmosConfig = cosmosConfig.Value;
        }
        public async Task<(IReadOnlyList<CmsItem> results, int total)> List(CmsType cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0, string? searchQuery = null)
        {
            var result = await _cosmosService.List(cmsType.ToString(), sortField, sortOrder, pageSize, pageIndex, searchQuery).ConfigureAwait(false);
            return (result.results.Select(x => x.ToCmsItem()).Where(x => x != null).Select(x => x!).ToList<CmsItem>(), result.total);
        }

        public Task Write<T>(T item, CmsType cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem
        {
            return _cosmosService.Write(item.ToCosmosCmsItem(), cmsType.ToString(), id, lang);
        }

        public Task Delete(CmsType cmsType, Guid id, string? lang, string? currentUser)
        {
            return _cosmosService.Delete(cmsType.ToString(), id, lang);

        }

        public async Task<T?> Read<T>(CmsType cmsType, Guid id, string? lang) where T : CmsItem
        {
            var result = await _cosmosService.Read(cmsType.ToString(), id, lang).ConfigureAwait(false);
            return result?.ToCmsItem<T>();
        }
    }
}
