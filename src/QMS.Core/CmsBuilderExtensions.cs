using QMS.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Core
{
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder AddCoreCms(this CmsBuilder builder)
        {
            builder.AddNamespace(typeof(HostingStartupModule).Namespace);
            return builder;
        }
    }
}
