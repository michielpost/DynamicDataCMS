using QMS.Core.Models;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Services
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
        private readonly IEnumerable<IWriteCmsItemInterceptor> writeCmsItemInterceptors;

        public DataProviderWrapperService(IReadFile readFileProvider,
           IEnumerable<IWriteFile> writeFileProviders,
           IReadCmsItem readCmsItemProvider,
           IEnumerable<IWriteCmsItem> writeCmsItemProviders,
           IEnumerable<IWriteCmsItemInterceptor> writeCmsItemInterceptors
           )
        {
            this.readFileProvider = readFileProvider;
            this.writeFileProviders = writeFileProviders;
            this.readCmsItemProvider = readCmsItemProvider;
            this.writeCmsItemProviders = writeCmsItemProviders;

            this.writeCmsItemInterceptors = writeCmsItemInterceptors;
        }


        public bool CanSort => readCmsItemProvider.CanSort;

        public Task<(IReadOnlyList<CmsItem> results, int total)> List(string cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0, string? searchQuery = null)
        {
            return readCmsItemProvider.List(cmsType, sortField, sortOrder, pageSize, pageIndex, searchQuery);
        }

        public Task<T?> Read<T>(string cmsType, Guid id, string? lang) where T : CmsItem
        {
           return readCmsItemProvider.Read<T>(cmsType, id, lang);
        }

        public Task<CmsFile?> ReadFile(string cmsType, Guid id, string fieldName, string? lang)
        {
            return readFileProvider.ReadFile(cmsType, id, fieldName, lang);
        }

        public async Task Write<T>(T item, string cmsType, Guid id, string? lang) where T : CmsItem
        {
            //Run interceptors before saving
            foreach(var interceptor in writeCmsItemInterceptors)
            {
               item = await interceptor.InterceptAsync(item, cmsType, id, lang).ConfigureAwait(false);
            };

            //Set id and type property, they might get lost during intercept
            item.Id = id;
            item.CmsType = cmsType;

            await Task.WhenAll(writeCmsItemProviders.Select(x => x.Write(item, cmsType, id, lang))).ConfigureAwait(false);
        }

        public Task Delete(string cmsType, Guid id, string? lang)
        {
            return Task.WhenAll(writeCmsItemProviders.Select(x => x.Delete(cmsType, id, lang)));
        }

        public async Task<string> WriteFile(CmsFile file, string cmsType, Guid id, string fieldName, string? lang)
        {
            var task = await Task.WhenAll(writeFileProviders.Select(x => x.WriteFile(file, cmsType, id, fieldName, lang))).ConfigureAwait(false);

            return task.First();
        }
    }
}
