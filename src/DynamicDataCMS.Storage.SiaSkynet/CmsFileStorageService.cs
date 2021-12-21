using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Interfaces;
using DynamicDataCMS.Storage.SiaSkynet.Models;
using Microsoft.Extensions.Options;
using SiaSkynet;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataCMS.Storage.SiaSkynet
{
    /// <summary>
    /// Implements read and write interface for files for Azure Blob Storage
    /// </summary>
    public class CmsFileStorageService : IReadFile, IWriteFile
    {
        private SiaSkynetClient _client;

        public CmsFileStorageService(IOptions<SkynetConfig> skynetConfig)
        {
            if (!string.IsNullOrEmpty(skynetConfig.Value.BaseUrl))
                _client = new SiaSkynetClient(skynetConfig.Value.BaseUrl);
            else
                _client = new SiaSkynetClient();
        }

        public async Task<CmsFile?> ReadFile(string fileName)
        {
            var response = await _client.DownloadFileAsByteArrayAsync(fileName);

            return new CmsFile { Bytes = response.file, ContentType = response.contentType };
        }

        public async Task<string> WriteFile(CmsFile file, CmsType cmsType, Guid id, string fieldName, string? lang, string? currentUser)
        {
            using (Stream stream = new MemoryStream(file.Bytes))
            {
                var response = await _client.UploadFileAsync($"{fieldName}.{file.ContentType}", stream);

                return response.Skylink;
            }
        }
    }
}
