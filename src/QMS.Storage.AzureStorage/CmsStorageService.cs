using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Storage.AzureStorage
{
    public class CmsStorageService
    {
        private readonly AzureStorageService azureStorageService;

        public CmsStorageService(AzureStorageService azureStorageService)
        {
            this.azureStorageService = azureStorageService;
        }

        public Task<ICloudBlob> StoreFileAsync(byte[] bytes, string mimeType, string cmsType, string id, string fieldName, string lang)
        {
            string fileName = $"{cmsType}/{id}/{lang}/{fieldName}";
            if(string.IsNullOrEmpty(lang))
                fileName = $"{cmsType}/{id}/{fieldName}";

            return azureStorageService.StoreFileAsync(bytes, mimeType, fileName);
        }
    }
}
