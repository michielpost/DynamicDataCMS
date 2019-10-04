using Microsoft.Azure.Storage.Blob;
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

        public bool CanSort => false;

        public CmsItemStorageService(AzureStorageService azureStorageService)
        {
            this.azureStorageService = azureStorageService;
        }

        public async Task<(IReadOnlyList<CmsItem> results, int total)> List(string cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0)
        {
            var directoryInfo = await azureStorageService.GetFilesFromDirectory(cmsType).ConfigureAwait(false);

            List<CmsItem> result = new List<CmsItem>();

            foreach(var file in directoryInfo.Skip(pageSize * pageIndex).Take(pageSize))
            {
                if (file is CloudBlockBlob cloudBlockBlob)
                {
                    string fileName = cloudBlockBlob.Name
                        .Replace($"{cmsType}/", "")
                        .Replace(".json", "");
                    var cmsItem = await Read(cmsType, fileName, null).ConfigureAwait(false);
                    if(cmsItem != null)
                        result.Add(cmsItem);
                }
            }

            var total = directoryInfo.Count();

            return (result, total);
        }

        public async Task<CmsItem?> Read(string cmsType, string id, string? lang)
        {
            var fileName = GenerateFileName(cmsType, id, lang);

            try
            {
                var blob = await azureStorageService.GetFileReference(fileName).ConfigureAwait(false);

                using (var stream = new MemoryStream())
                {
                    // download image
                    await blob.DownloadToStreamAsync(stream).ConfigureAwait(false);
                    var fileBytes = stream.ToArray();

                    string json = Encoding.ASCII.GetString(fileBytes);

                    var cmsItem = JsonSerializer.Deserialize<CmsItem>(json);

                    return cmsItem;
                }
            }
            catch(FileNotFoundException)
            {
                return null;
            }
        }

        public Task Write(CmsItem item, string cmsType, string id, string? lang)
        {
            var fileName = GenerateFileName(cmsType, id, lang);
            var json = JsonSerializer.Serialize(item);

            byte[] fileData = Encoding.ASCII.GetBytes(json);

            return azureStorageService.StoreFileAsync(fileData, "application/json", fileName);
        }

        public async Task Delete(string cmsType, string id, string? lang)
        {
            var fileName = GenerateFileName(cmsType, id, null);
            
            await azureStorageService.DeleteFileAsync(fileName).ConfigureAwait(false);

            //Get translations
            var files = await azureStorageService.GetFilesFromDirectory($"{cmsType}/{id}").ConfigureAwait(false);
            foreach(var file in files)
            {
                if (file is CloudBlockBlob cloudBlockBlob)
                {
                    await cloudBlockBlob.DeleteAsync().ConfigureAwait(false);
                }
            }
        }

        private static string GenerateFileName(string cmsType, string id, string? lang)
        {
            string fileName = $"{cmsType}/{id}.json";
            if(!string.IsNullOrEmpty(lang))
                fileName = $"{cmsType}/{id}/{lang}.json";
            return fileName;
        }
    }
}
