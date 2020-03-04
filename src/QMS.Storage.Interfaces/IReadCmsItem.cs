using QMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QMS.Storage.Interfaces
{
    /// <summary>
    /// Interface that defines Read operations on CmsItems
    /// </summary>
    public interface IReadCmsItem : ICmsItemBase
    {
        bool CanSort(string cmsType);

        Task<T?> Read<T>(string cmsType, Guid id, string? lang) where T : CmsItem;
        Task<(IReadOnlyList<CmsItem> results, int total)> List(string cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0, string? searchQuery = null);
    }

   
}
