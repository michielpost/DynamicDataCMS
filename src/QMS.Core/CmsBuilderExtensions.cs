using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QMS.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Core
{
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureCoreCms(this CmsBuilder builder)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            services.AddTransient<IPostConfigureOptions<StaticFileOptions>, CmsConfigureOptions>();

            return builder;

        }
    }
}
