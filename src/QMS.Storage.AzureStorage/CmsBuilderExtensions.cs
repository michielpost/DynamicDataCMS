using QMS.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Storage.AzureStorage
{
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder AddAzureStorage(this CmsBuilder builder)
        {
            builder.AddNamespace(typeof(HostingStartupModule).Namespace);
            return builder;
        }
    }
}
