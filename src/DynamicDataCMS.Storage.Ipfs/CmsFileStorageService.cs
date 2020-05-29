using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Interfaces;
using Ipfs.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataCMS.Storage.Ipfs
{
    /// <summary>
    /// Implements read and write interface for files for Azure Blob Storage
    /// </summary>
    public class CmsFileStorageService : IReadFile, IWriteFile
    {
        private IpfsClient _client;

        public CmsFileStorageService()
        {
            _client = new IpfsClient();
        }

        public async Task<CmsFile?> ReadFile(string fileName)
        {
            var response = await _client.FileSystem.ReadFileAsync(fileName);
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

                return response.ToLink().Name;
            }
        }

    }
}
