using Microsoft.Extensions.Options;
using DynamicDataCMS.Storage.AzureStorage.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace DynamicDataCMS.Storage.AzureStorage
{
    /// <summary>
    /// Azure Storage helper service
    /// </summary>
    public class AzureStorageService
    {
        private readonly AzureStorageConfig _config;

        public AzureStorageService(IOptions<AzureStorageConfig> config)
        {
            _config = config.Value;
        }

        /// <summary>
        /// Store the given fileData in the blobstorage and return a blob identifying the results
        /// </summary>
        public async Task<BlobClient> StoreFileAsync(byte[] fileData, string blobName, string? contentType, string? containerName = null)
        {
            var blobContainer = await GetBlobContainer(containerName).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentNullException(nameof(blobName));

            var blobClient = blobContainer.GetBlobClient(blobName);

            using (var stream = new MemoryStream(fileData, writable: false))
            {
                await blobClient.UploadAsync(stream,
                  new BlobHttpHeaders()
                  {
                      ContentType = contentType
                  });
            }

            return blobClient;
        }

        /// <summary>
        /// retrieve a new pointer to the blobstore container containing our assets
        /// </summary>
        private async Task<BlobContainerClient> GetBlobContainer(string? containerName)
        {
            if (string.IsNullOrEmpty(containerName))
                containerName = _config.AssetsContainerName;

            var storageAccount = new BlobServiceClient(_config.ConnectionString);

            BlobContainerClient container = storageAccount.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.None).ConfigureAwait(false);

            return container;
        }

        /// <summary>
        /// Delete the given file from the blobstore
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task DeleteFileAsync(string blobName, string? containerName = null)
        {
            var blobReference = await GetFileReference(blobName, containerName).ConfigureAwait(false);

            await blobReference.DeleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Delete the given file from the blobstore
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task<BlobClient> GetFileReference(string blobName, string? containerName = null)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentNullException(nameof(blobName));

            var container = await GetBlobContainer(containerName).ConfigureAwait(false);

            var blobReference = container.GetBlobClient(blobName);
            if (blobReference == null)
                throw new ArgumentException($"Given blobName '{blobName}' does not exist");

            return blobReference;
        }

        public async Task<(T? file, DateTimeOffset? createdDate)> ReadFileAsJsonAsync<T>(string fileName) where T : class
        {
            try
            {
                var blob = await GetFileReference(fileName).ConfigureAwait(false);

                using (var stream = new MemoryStream())
                {
                    // download image
                    await blob.DownloadToAsync(stream).ConfigureAwait(false);
                    var fileBytes = stream.ToArray();

                    string json = Encoding.ASCII.GetString(fileBytes);

                    var cmsItem = JsonSerializer.Deserialize<T>(json);

                    var props = await blob.GetPropertiesAsync().ConfigureAwait(false);

                    return (cmsItem, props.Value.CreatedOn);
                }
            }
            catch (Azure.RequestFailedException)
            {
                return default;
            }
        }

        public Task WriteFileAsJson<T>(T item, string fileName)
        {
            var json = JsonSerializer.Serialize(item);

            byte[] fileData = Encoding.ASCII.GetBytes(json);

            return StoreFileAsync(fileData, fileName, "application/json");
        }

        public async Task<IEnumerable<BlobItem>> GetFilesFromDirectory(string path, string? containerName = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var container = await GetBlobContainer(containerName).ConfigureAwait(false);

            var dirReference = container.GetBlobsByHierarchyAsync(Azure.Storage.Blobs.Models.BlobTraits.All, Azure.Storage.Blobs.Models.BlobStates.All, path);
            if (dirReference == null)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Given directory '{0}' does not exist",
                  path));

            var list = new List<BlobItem>();
            await foreach (BlobHierarchyItem blobHierarchyItem in container.GetBlobsByHierarchyAsync(prefix: path, delimiter: "/"))
            {
                if (blobHierarchyItem.IsBlob)
                    list.Add(blobHierarchyItem.Blob);
            }

            return list;
        }
    }
}
