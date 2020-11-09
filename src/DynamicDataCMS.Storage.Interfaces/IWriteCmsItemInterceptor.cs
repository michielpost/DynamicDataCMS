using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DynamicDataCMS.Core.Models;

namespace DynamicDataCMS.Storage.Interfaces
{
    public interface IWriteCmsItemInterceptor : ICmsItemBase
    {
        Task<T> InterceptAsync<T>(T item, CmsType cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem;
    }
}
