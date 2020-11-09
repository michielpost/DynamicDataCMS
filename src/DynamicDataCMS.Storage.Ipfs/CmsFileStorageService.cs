using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Interfaces;
using DynamicDataCMS.Storage.Ipfs.Models;
using Ipfs.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicDataCMS.Storage.Ipfs
{
    /// <summary>
    /// Implements read and write interface for files for Azure Blob Storage
    /// </summary>
    public class CmsFileStorageService : IReadFile, IWriteFile
    {
        private IpfsClient _client;

        public CmsFileStorageService(IOptions<IpfsConfig> ipfsConfig)
        {
            if (!string.IsNullOrEmpty(ipfsConfig.Value.Host))
                _client = new IpfsClient(ipfsConfig.Value.Host);
            else
                _client = new IpfsClient();
        }

        public async Task<CmsFile?> ReadFile(string fileName)
        {
            //FileSystem.ReadFileAsyn does a GET, but should be a POST
            //var response = await _client.FileSystem.ReadFileAsync(fileName);
            var response = await _client.PostDownloadAsync("cat", default(CancellationToken), fileName);
            using (MemoryStream ms = new MemoryStream())
            {
                response.CopyTo(ms);
                return new CmsFile { Bytes = ms.ToArray(), ContentType = string.Empty };

            }

        }

        public async Task<string> WriteFile(CmsFile file, CmsType cmsType, Guid id, string fieldName, string? lang, string? currentUser)
        {
            using (Stream stream = new MemoryStream(file.Bytes))
            {
                var response = await _client.FileSystem.AddAsync(stream, fieldName);

                return response.ToLink().Id.ToString();
            }
        }

    }
}
