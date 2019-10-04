using QMS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QMS.Storage.Interfaces
{
    /// <summary>
    /// Interface that defines Read operations on CmsItems
    /// </summary>
    public interface IReadCmsItem
    {
        bool CanSort { get; }

        Task<CmsItem?> Read(string cmsType, string id, string? lang);
        Task<(IReadOnlyList<CmsItem> results, int total)> List(string cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0);
    }

   
}
