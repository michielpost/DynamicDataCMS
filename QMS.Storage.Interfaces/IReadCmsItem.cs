using QMS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QMS.Storage.Interfaces
{
    public interface IReadCmsItem
    {
        Task<CmsItem> Read(string cmsType, string id);
        Task<IReadOnlyList<CmsItem>> List(string cmsType);
    }

   
}
