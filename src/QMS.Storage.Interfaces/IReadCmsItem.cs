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
        Task<CmsItem> Read(string cmsType, string id);
        Task<IReadOnlyList<CmsItem>> List(string cmsType);
    }

   
}
