using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using QMS.Models;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QMS.Storage.AzureStorage
{
    public class CmsItemStorageService : IReadCmsItem, IWriteCmsItem
    {
        private readonly AzureStorageService azureStorageService;
        private readonly CmsConfiguration cmsConfiguration;

        public bool CanSort => true;

        public CmsItemStorageService(AzureStorageService azureStorageService, IOptions<CmsConfiguration> cmsConfiguration)
        {
            this.azureStorageService = azureStorageService;
            this.cmsConfiguration = cmsConfiguration.Value;
        }

        public async Task<(IReadOnlyList<CmsItem> results, int total)> List(string cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0, string? searchQuery = null)
        {
            var typeInfo = cmsConfiguration.Entities.Where(x => x.Key == cmsType).FirstOrDefault();

            //Get index file
            var indexFileName = GenerateFileName(cmsType, "_index", null);

            //Get current index file
            var indexFile = await azureStorageService.ReadFileAsJson<List<CmsItem>>(indexFileName).ConfigureAwait(false);
            indexFile = indexFile ?? new List<CmsItem>();

            var returnItems = indexFile.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                returnItems = returnItems.Where(x => string.Join(" ", x.AdditionalProperties.Values.Select(x => x.ToString().ToLowerInvariant())).Contains(searchQuery));
            }

            if (sortField != null)
            {
                sortOrder = sortOrder ?? "Asc";
                if (sortOrder == "Asc")
                    returnItems = returnItems.OrderBy(x => x.AdditionalProperties[sortField].ToString());
                else
                    returnItems = returnItems.OrderByDescending(x => x.AdditionalProperties[sortField].ToString());
            }

            return (returnItems.Skip(pageSize * pageIndex).Take(pageSize).ToList(), indexFile.Count);

            //var directoryInfo = await azureStorageService.GetFilesFromDirectory(cmsType).ConfigureAwait(false);

            //List<CmsItem> result = new List<CmsItem>();

            //foreach(var file in directoryInfo.Skip(pageSize * pageIndex).Take(pageSize))
            //{
            //    if (file is CloudBlockBlob cloudBlockBlob)
            //    {
            //        string fileName = cloudBlockBlob.Name
            //            .Replace($"{cmsType}/", "")
            //            .Replace(".json", "");
            //        var cmsItem = await Read(cmsType, fileName, null).ConfigureAwait(false);
            //        if(cmsItem != null)
            //            result.Add(cmsItem);
            //    }
            //}

            //var total = directoryInfo.Count();

            //return (result, total);
        }

        public Task<T?> Read<T>(string cmsType, Guid id, string? lang) where T : CmsItem
        {
            var fileName = GenerateFileName(cmsType, id, lang);

            return azureStorageService.ReadFileAsJson<T>(fileName);
        }

        public async Task Write<T>(T item, string cmsType, Guid id, string? lang) where T : CmsItem
        {
            var fileName = GenerateFileName(cmsType, id, lang);
            await azureStorageService.WriteFileAsJson(item, fileName);

            //Write index file for paging and sorting
            var indexFileName = GenerateFileName(cmsType, "_index", lang);
            var typeInfo = cmsConfiguration.Entities.Where(x => x.Key == cmsType).FirstOrDefault();

            if (typeInfo == null)
                return;

            //Get current index file
            var indexFile = await azureStorageService.ReadFileAsJson<List<CmsItem>>(indexFileName).ConfigureAwait(false);
            indexFile = indexFile ?? new List<CmsItem>();

            //Remove existing item
            indexFile.Remove(indexFile.Where(x => x.Id == item.Id).FirstOrDefault());

            var indexItem = new CmsItem { 
                Id = id, 
                CmsType = cmsType, 
                LastModifiedDate = item.LastModifiedDate 
            };

            foreach (var prop in typeInfo.ListViewProperties)
            {
                var value = item.AdditionalProperties[prop.Key];
                indexItem.AdditionalProperties[prop.Key] = value;
            }

            indexFile.Add(indexItem);

            await azureStorageService.WriteFileAsJson(indexFile, indexFileName);
        }



        public async Task Delete(string cmsType, Guid id, string? lang)
        {
            var fileName = GenerateFileName(cmsType, id, null);

            await azureStorageService.DeleteFileAsync(fileName).ConfigureAwait(false);

            //Get translations
            var files = await azureStorageService.GetFilesFromDirectory($"{cmsType}/{id}").ConfigureAwait(false);
            foreach (var file in files)
            {
                if (file is CloudBlockBlob cloudBlockBlob)
                {
                    await cloudBlockBlob.DeleteAsync().ConfigureAwait(false);
                }
            }

            //Write index file for paging and sorting
            var indexFileName = GenerateFileName(cmsType, "_index", lang);
            //Get current index file
            var indexFile = await azureStorageService.ReadFileAsJson<List<CmsItem>>(indexFileName).ConfigureAwait(false);
            indexFile = indexFile ?? new List<CmsItem>();

            //Remove existing item
            indexFile.Remove(indexFile.Where(x => x.Id == id).FirstOrDefault());
            await azureStorageService.WriteFileAsJson(indexFile, indexFileName);
        }

        private static string GenerateFileName(string cmsType, Guid id, string? lang)
        {
            return GenerateFileName(cmsType, id.ToString(), lang);
        }
        private static string GenerateFileName(string cmsType, string id, string? lang)
        {
            string fileName = $"{cmsType}/{id}.json";
            if (!string.IsNullOrEmpty(lang))
                fileName = $"{cmsType}/{id}/{lang}.json";
            return fileName;
        }
    }
}
