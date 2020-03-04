using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DynamicDataCms.Core.Models;

namespace DynamicDataCms.Storage.Interfaces
{
    public interface IWriteCmsItemInterceptor
    {
        Task<T> InterceptAsync<T>(T item, string cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem;
    }
}
