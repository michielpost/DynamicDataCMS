using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataCMS.Storage.AzureStorage
{
    /// <summary>
    /// Implements read and write interface for files for Azure Blob Storage
    /// </summary>
    public class CmsFileStorageService : IReadFile, IWriteFile
    {
        private readonly AzureStorageService azureStorageService;

        public CmsFileStorageService(AzureStorageService azureStorageService)
        {
            this.azureStorageService = azureStorageService;
        }

        public async Task<CmsFile?> ReadFile(string fileName)
        {
            // get original file
            var blob = await azureStorageService.GetFileReference(fileName).ConfigureAwait(false);

            if ((!await blob.ExistsAsync().ConfigureAwait(false)))
                return null;

            using (var stream = new MemoryStream())
            {
                // download file
                await blob.DownloadToStreamAsync(stream).ConfigureAwait(false);
                var fileBytes = stream.ToArray();

                return new CmsFile { Bytes = fileBytes, ContentType = blob.Properties.ContentType };
            }
        }

        public async Task<string> WriteFile(CmsFile file, CmsType cmsType, Guid id, string fieldName, string? lang, string? currentUser)
        {
            string fileName = GenerateFileName(cmsType, id, fieldName, lang);

            var blob = await azureStorageService.StoreFileAsync(file.Bytes, file.ContentType, fileName).ConfigureAwait(false);

            return fileName;
        }

        private static string GenerateFileName(CmsType cmsType, Guid id, string fieldName, string? lang)
        {
            string fileName = $"{cmsType}/{id}/{lang}/{fieldName}";
            if (string.IsNullOrEmpty(lang))
                fileName = $"{cmsType}/{id}/{fieldName}";
            return fileName;
        }
    }
}
