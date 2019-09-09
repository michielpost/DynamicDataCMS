using QMS.Models;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Services
{
    /// <summary>
    /// Wraps dataprovider calls because there can be multiple interface implementations
    /// Always read from one source, write to all sources
    /// </summary>
    public class DataProviderWrapperService :
        IReadFile,
        IWriteFile,
        IReadCmsItem,
        IWriteCmsItem
    {
        private readonly IReadFile readFileProvider;
        private readonly IEnumerable<IWriteFile> writeFileProviders;
        private readonly IReadCmsItem readCmsItemProvider;
        private readonly IEnumerable<IWriteCmsItem> writeCmsItemProviders;

        public DataProviderWrapperService(IReadFile readFileProvider,
            IEnumerable<IWriteFile> writeFileProviders,
            IReadCmsItem readCmsItemProvider,
            IEnumerable<IWriteCmsItem> writeCmsItemProviders)
        {
            this.readFileProvider = readFileProvider;
            this.writeFileProviders = writeFileProviders;
            this.readCmsItemProvider = readCmsItemProvider;
            this.writeCmsItemProviders = writeCmsItemProviders;
        }


        public Task<IReadOnlyList<CmsItem>> List(string cmsType)
        {
            return readCmsItemProvider.List(cmsType);
        }

        public Task<CmsItem> Read(string cmsType, string id)
        {
           return readCmsItemProvider.Read(cmsType, id);
        }

        public Task<byte[]> ReadFile(string cmsType, string id, string fieldName, string lang)
        {
            return readFileProvider.ReadFile(cmsType, id, fieldName, lang);
        }

        public Task Write(CmsItem item, string cmsType, string id, string lang)
        {
            return Task.WhenAll(writeCmsItemProviders.Select(x => x.Write(item, cmsType, id, lang)));
        }

        public Task Delete(string cmsType, string id)
        {
            return Task.WhenAll(writeCmsItemProviders.Select(x => x.Delete(cmsType, id)));
        }

        public async Task<string> WriteFile(byte[] bytes, string mimeType, string cmsType, string id, string fieldName, string lang)
        {
            var task = await Task.WhenAll(writeFileProviders.Select(x => x.WriteFile(bytes, mimeType, cmsType, id, fieldName, lang))).ConfigureAwait(false);

            return task.First();
        }
    }
}
