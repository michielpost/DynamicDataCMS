using QMS.Models;
using QMS.Storage.Interfaces;
using QMS.Web.Models.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace QMS.Web.Interceptor
{
    public class ExampleInterceptor : IWriteCmsItemInterceptor
    {
        public void Intercept<T>(T item, string cmsType, Guid id, string? lang) where T : CmsItem
        {

        }
    }
}
