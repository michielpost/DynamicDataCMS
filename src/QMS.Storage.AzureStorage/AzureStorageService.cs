using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using QMS.Storage.AzureStorage.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QMS.Storage.AzureStorage
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
        public async Task<ICloudBlob> StoreFileAsync(byte[] fileData, string contentType, string? fileName = null, string? containerName = null, string? leaseId = null)
        {
            var blobContainer = await GetBlobContainer(containerName).ConfigureAwait(false);
            if (string.IsNullOrEmpty(fileName))
                fileName = Guid.NewGuid().ToString();

            var blockBlob = blobContainer.GetBlockBlobReference(fileName);

            blockBlob.Properties.ContentType = contentType;
            //blockBlob.Properties.CacheControl = "public, max-age=31536000"; //Cache for 1 year

            using (var stream = new MemoryStream(fileData, writable: false))
            {
                if(leaseId != null)
                    await blockBlob.UploadFromStreamAsync(stream, new AccessCondition { LeaseId = leaseId }, null, null).ConfigureAwait(false);
                else
                    await blockBlob.UploadFromStreamAsync(stream).ConfigureAwait(false);
            }

            return blockBlob;
        }

        /// <summary>
        /// retrieve a new pointer to the blobstore container containing our assets
        /// </summary>
        private async Task<CloudBlobContainer> GetBlobContainer(string? containerName)
        {
            if (string.IsNullOrEmpty(containerName))
                containerName = _config.ContainerName;

            var storageAccount = CloudStorageAccount.Parse(_config.StorageAccount);

            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);
            await blobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);
            return blobContainer;
        }

        /// <summary>
        /// Delete the given file from the blobstore
        /// </summary>
        /// <param name="blobStoreId"></param>
        /// <returns></returns>
        public async Task DeleteFileAsync(string blobStoreId, string? containerName = null)
        {
            if (string.IsNullOrWhiteSpace(blobStoreId))
                throw new ArgumentNullException(nameof(blobStoreId));

            var container = await GetBlobContainer(containerName).ConfigureAwait(false);

            var blobReference = await container.GetBlobReferenceFromServerAsync(blobStoreId).ConfigureAwait(false);
            if (blobReference == null)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Given blobStoreId '{0}' does not exist",
                  blobStoreId));

            await blobReference.DeleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get file from the blobstore
        /// </summary>
        /// <param name="blobStoreId"></param>
        /// <returns></returns>
        public async Task<ICloudBlob> GetFileReference(string blobStoreId, string? containerName = null)
        {
            if (string.IsNullOrWhiteSpace(blobStoreId))
                throw new ArgumentNullException(nameof(blobStoreId));

            var container = await GetBlobContainer(containerName).ConfigureAwait(false);

            var blobReference = container.GetBlockBlobReference(blobStoreId);
            if (blobReference == null)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Given blobStoreId '{0}' does not exist",
                  blobStoreId));

            if (!await blobReference.ExistsAsync().ConfigureAwait(false))
                throw new FileNotFoundException();

            if (blobReference.Properties.Length == 0)
            {
                throw new InvalidDataException();
            }

            return blobReference;
        }

        public async Task<(T?, string? leaseId)> ReadFileAsJson<T>(string fileName, TimeSpan? leaseTime = null) where T : class
        {
            string? leaseId = null;
            ICloudBlob? blob = null;
            try
            {
                blob = await GetFileReference(fileName).ConfigureAwait(false);

                if (leaseTime.HasValue)
                    leaseId = await blob.AcquireLeaseAsync(leaseTime);

                using (var stream = new MemoryStream())
                {
                    // download image
                    await blob.DownloadToStreamAsync(stream).ConfigureAwait(false);
                    var fileBytes = stream.ToArray();

                    string json = Encoding.ASCII.GetString(fileBytes);

                    var cmsItem = JsonSerializer.Deserialize<T>(json);

                    return (cmsItem, leaseId);
                }
            }
            catch (FileNotFoundException)
            {
                return default;
            }
            finally
            {
                if (leaseId != null && blob != null)
                    await blob.ReleaseLeaseAsync(new AccessCondition { LeaseId = leaseId });
            }
        }

        public Task WriteFileAsJson<T>(T item, string fileName, string? leaseId = null)
        {
            var json = JsonSerializer.Serialize(item);

            byte[] fileData = Encoding.ASCII.GetBytes(json);

            return StoreFileAsync(fileData, "application/json", fileName, leaseId: leaseId);
        }

        public async Task<IEnumerable<IListBlobItem>> GetFilesFromDirectory(string path, string? containerName = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var container = await GetBlobContainer(containerName).ConfigureAwait(false);

            var dirReference = container.GetDirectoryReference(path);
            if (dirReference == null)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Given directory '{0}' does not exist",
                  path));

            var list = dirReference.ListBlobs();

            return list;
        }
    }
}
