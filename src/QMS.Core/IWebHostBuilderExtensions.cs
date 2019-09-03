using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMS.Core
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseQms(this IWebHostBuilder builder, CmsBuilder cmsBuilder)
        {
            if (cmsBuilder == null || !cmsBuilder.HasDataPackages)
                throw new Exception("CMS is not configured with any data storage packages");

            builder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, cmsBuilder.GetHostingStartupAssembliesKey());

            return builder;
        }
    }
}
