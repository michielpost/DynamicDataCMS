using DynamicDataCMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataCMS.Storage.Interfaces
{
    /// <summary>
    /// Interface that defines write operations on CmsItems
    /// </summary>
    public interface IWriteCmsItem : ICmsItemBase
    {
        Task Write<T>(T item, CmsType cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem;
        Task Delete(CmsType cmsType, Guid id, string? lang, string? currentUser);
    }
}
