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
    /// </summary>
    public class DataProviderWrapperService :
        IReadFile,
        IWriteFile,
        IReadCmsItem,
        IWriteCmsItem
    {
        private readonly IEnumerable<IReadFile> readFileProviders;
        private readonly IEnumerable<IWriteFile> writeFileProviders;
        private readonly IEnumerable<IReadCmsItem> readCmsItemProviders;
        private readonly IEnumerable<IWriteCmsItem> writeCmsItemProviders;

        public DataProviderWrapperService(IEnumerable<IReadFile> readFileProviders,
            IEnumerable<IWriteFile> writeFileProviders,
            IEnumerable<IReadCmsItem> readCmsItemProviders,
            IEnumerable<IWriteCmsItem> writeCmsItemProviders)
        {
            this.readFileProviders = readFileProviders;
            this.writeFileProviders = writeFileProviders;
            this.readCmsItemProviders = readCmsItemProviders;
            this.writeCmsItemProviders = writeCmsItemProviders;
        }


        public async Task<IReadOnlyList<CmsItem>> List(string cmsType)
        {
            var task = await Task.WhenAny(readCmsItemProviders.Select(x => x.List(cmsType))).ConfigureAwait(false);

            return await task.ConfigureAwait(false);
        }

        public async Task<CmsItem> Read(string cmsType, string id)
        {
            var task = await Task.WhenAny(readCmsItemProviders.Select(x => x.Read(cmsType, id))).ConfigureAwait(false);

            return await task.ConfigureAwait(false);
        }

        public async Task<byte[]> ReadFile(string cmsType, string id, string fieldName, string lang)
        {
            var task = await Task.WhenAny(readFileProviders.Select(x => x.ReadFile(cmsType, id, fieldName, lang))).ConfigureAwait(false);

            return await task.ConfigureAwait(false);
        }

        public Task Write(CmsItem item, string cmsType, string id, string lang)
        {
            return Task.WhenAll(writeCmsItemProviders.Select(x => x.Write(item, cmsType, id, lang)));
        }

        public async Task<Uri> WriteFile(byte[] bytes, string mimeType, string cmsType, string id, string fieldName, string lang)
        {
            var task = await Task.WhenAll(writeFileProviders.Select(x => x.WriteFile(bytes, mimeType, cmsType, id, fieldName, lang))).ConfigureAwait(false);

            return task.First();
        }
    }
}
