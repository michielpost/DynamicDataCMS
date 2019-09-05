using Microsoft.Azure.Storage.Blob;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Storage.AzureStorage
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

        public async Task<byte[]> ReadFile(string cmsType, string id, string fieldName, string lang)
        {
            string fileName = GenerateFileName(cmsType, id, fieldName, lang);

            // get original image
            var blob = await azureStorageService.GetFileReference(fileName).ConfigureAwait(false);

            using (var stream = new MemoryStream())
            {
                // download image
                await blob.DownloadToStreamAsync(stream).ConfigureAwait(false);
                var imageBytes = stream.ToArray();

                return imageBytes;
            }
        }

        public async Task<string> WriteFile(byte[] bytes, string mimeType, string cmsType, string id, string fieldName, string lang)
        {
            string fileName = GenerateFileName(cmsType, id, fieldName, lang);

            var blob = await azureStorageService.StoreFileAsync(bytes, mimeType, fileName).ConfigureAwait(false);

            return fileName;
        }

        private static string GenerateFileName(string cmsType, string id, string fieldName, string lang)
        {
            string fileName = $"{cmsType}/{id}/{lang}/{fieldName}";
            if (string.IsNullOrEmpty(lang))
                fileName = $"{cmsType}/{id}/{fieldName}";
            return fileName;
        }
    }
}
