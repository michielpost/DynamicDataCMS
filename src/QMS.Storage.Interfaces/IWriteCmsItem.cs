using QMS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Storage.Interfaces
{
    /// <summary>
    /// Interface that defines write operations on CmsItems
    /// </summary>
    public interface IWriteCmsItem
    {
        Task Write(CmsItem item, string cmsType, string id, string? lang);
        Task Delete(string cmsType, string id, string? lang);
    }
}
