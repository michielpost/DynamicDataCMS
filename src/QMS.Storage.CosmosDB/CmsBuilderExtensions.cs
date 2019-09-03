using QMS.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Storage.CosmosDB
{
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder AddCosmosDB(this CmsBuilder builder)
        {
            builder.AddNamespace(typeof(HostingStartupModule).Namespace);
            return builder;
        }
    }
}
