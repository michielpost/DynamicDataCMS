using Microsoft.Extensions.Options;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SiaSkynet;
using DynamicDataCMS.Storage.SiaSkynet.Models;

namespace DynamicDataCMS.Storage.SiaSkynet
{
    public class CmsItemStorageService : IReadCmsItem, IWriteCmsItem
    {
        private readonly CmsConfiguration cmsConfiguration;
        private readonly SkynetConfig skynetConfig;
        private readonly byte[] privateKey;
        private readonly byte[] publicKey;
        private readonly SiaSkynetClient _client;

        public bool CanSort(CmsType cmsType) => true;
        public bool HandlesType(CmsType cmsType) => !skynetConfig.ExcludedTypes.Contains(cmsType);


        public CmsItemStorageService(IOptions<SkynetConfig> skynetConfig, IOptions<CmsConfiguration> cmsConfiguration)
        {
            this.skynetConfig = skynetConfig.Value;
            this.cmsConfiguration = cmsConfiguration.Value;

            if (!string.IsNullOrEmpty(skynetConfig.Value.BaseUrl))
                _client = new SiaSkynetClient(skynetConfig.Value.BaseUrl);
            else
                _client = new SiaSkynetClient();

            if (string.IsNullOrWhiteSpace(this.skynetConfig.Secret))
                throw new ArgumentNullException("SkynetConfig.Seed should contain a seed value to generate a private/public key pair.", nameof(skynetConfig));

            var keypair = SiaSkynetClient.GenerateKeys(this.skynetConfig.Secret);
            privateKey = keypair.privateKey;
            publicKey = keypair.publicKey;
        }

        public async Task<(IReadOnlyList<CmsItem> results, int total)> List(CmsType cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0, string? searchQuery = null)
        {
            List<CmsItem>? indexFile = await GetIndexFile(cmsType).ConfigureAwait(false);

            var returnItems = indexFile.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                returnItems = returnItems
                    .Where(x => string.Join(" ", x.AdditionalProperties.Values.Select(x => x.ToString())
                        .Where(x => x != null)
                        .Select(x => x!.ToLowerInvariant())
                    ).Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase));
            }

            if (sortField != null)
            {
                sortOrder = sortOrder ?? "Asc";
                if (sortOrder == "Asc")
                    returnItems = returnItems.OrderBy(x => x.AdditionalProperties[sortField].ToString());
                else
                    returnItems = returnItems.OrderByDescending(x => x.AdditionalProperties[sortField].ToString());
            }
            else
            {
                //Default sort by CreatedDate
                returnItems = returnItems.OrderBy(x => x.CreatedDate);
            }

            return (returnItems.Skip(pageSize * pageIndex).Take(pageSize).ToList(), indexFile.Count);
        }

        private async Task<List<CmsItem>> GetIndexFile(CmsType cmsType)
        {
            //Get index file
            var indexFileName = GenerateFileName(cmsType, "_index", null);

            //Get current index file
            var json = await _client.SkyDbGetAsString(publicKey, new RegistryKey(indexFileName)).ConfigureAwait(false);
            var indexFile = new List<CmsItem>();
            if (json != null)
            {
                indexFile = JsonSerializer.Deserialize<List<CmsItem>>(json);
                indexFile = indexFile ?? new List<CmsItem>();
            }

            return indexFile;
        }

        public async Task<T?> Read<T>(CmsType cmsType, Guid id, string? lang) where T : CmsItem
        {

            var fileName = GenerateFileName(cmsType, id, lang);

            var json = await _client.SkyDbGetAsString(publicKey, new RegistryKey(fileName)).ConfigureAwait(false);
            T? file = null;
            if (json != null)
            {
                file = JsonSerializer.Deserialize<T>(json);
            }

            //TODO: Something with creation date?

            return file;
        }

        public async Task Write<T>(T item, CmsType cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem
        {
            
            var fileName = GenerateFileName(cmsType, id, lang);
            string itemJson = JsonSerializer.Serialize(item);
            await _client.SkyDbSetAsString(privateKey, publicKey, new RegistryKey(fileName), itemJson).ConfigureAwait(false);

            //Write index file for paging and sorting
            var indexFileName = GenerateFileName(cmsType, "_index", lang);
            var typeInfo = cmsConfiguration.MenuItems.Where(x => x.Key == cmsType.Value).FirstOrDefault();

            if (typeInfo == null)
                return;

            //Get current index file
            List<CmsItem>? indexFile = await GetIndexFile(cmsType).ConfigureAwait(false);

            //Remove existing item
            CmsItem? oldItem = indexFile.Where(x => x.Id == item.Id).FirstOrDefault();
            if(oldItem != null)
                indexFile.Remove(oldItem);

            var indexItem = new CmsItem
            {
                Id = id,
                CmsType = cmsType,
                LastModifiedBy = item.LastModifiedBy,
                LastModifiedDate = item.LastModifiedDate
            };

            if (oldItem != null)
                indexItem.CreatedDate = oldItem.CreatedDate;

            foreach (var prop in typeInfo.ListViewProperties)
            {
                var value = item.AdditionalProperties[prop.Key];
                indexItem.AdditionalProperties[prop.Key] = value;
            }

            indexFile.Add(indexItem);

            string indexFileJson = JsonSerializer.Serialize(indexFile);
            await _client.SkyDbSetAsString(privateKey, publicKey, new RegistryKey(indexFileName), indexFileJson).ConfigureAwait(false);
        }

        public async Task Delete(CmsType cmsType, Guid id, string? lang, string? currentUser)
        {
            //Unable to delete actual files from Skynet

            //Get current index file
            List<CmsItem>? indexFile = await GetIndexFile(cmsType).ConfigureAwait(false);

            //Remove existing item
            var existing = indexFile.Where(x => x.Id == id).FirstOrDefault();
            if (existing != null)
            {
                indexFile.Remove(existing);

                string indexFileJson = JsonSerializer.Serialize(indexFile);
                var indexFileName = GenerateFileName(cmsType, "_index", lang);
                await _client.SkyDbSetAsString(privateKey, publicKey, new RegistryKey(indexFileName), indexFileJson).ConfigureAwait(false);
            }
        }

        public static string GenerateFileName(CmsType cmsType, Guid id, string? lang)
        {
            return GenerateFileName(cmsType, id.ToString(), lang);
        }
        public static string GenerateFileName(CmsType cmsType, string id, string? lang)
        {
            string fileName = $"{cmsType}_{id}.json";
            if (!string.IsNullOrEmpty(lang))
                fileName = $"{cmsType}_{id}_{lang}.json";
            return fileName;
        }
    }
}
