using System;
using System.Collections.Generic;
using System.Text;
using QMS.Models;

namespace QMS.Storage.Interfaces
{
    public interface IWriteCmsItemInterceptor
    {
        void Intercept<T>(T item, string cmsType, Guid id, string? lang) where T : CmsItem;
    }
}
