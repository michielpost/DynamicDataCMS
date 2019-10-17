using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using QMS.Core.Models;

namespace QMS.Storage.Interfaces
{
    public interface IWriteCmsItemInterceptor
    {
        Task InterceptAsync<T>(T item, string cmsType, Guid id, string? lang) where T : CmsItem;
    }
}
