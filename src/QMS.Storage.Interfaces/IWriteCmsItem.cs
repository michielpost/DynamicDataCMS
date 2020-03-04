using QMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Storage.Interfaces
{
    /// <summary>
    /// Interface that defines write operations on CmsItems
    /// </summary>
    public interface IWriteCmsItem : ICmsItemBase
    {
        Task Write<T>(T item, string cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem;
        Task Delete(string cmsType, Guid id, string? lang, string? currentUser);
    }
}
