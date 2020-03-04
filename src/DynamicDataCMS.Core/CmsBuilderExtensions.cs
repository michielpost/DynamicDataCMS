using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DynamicDataCms.Core;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCms.Core
{
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureCoreCms(this CmsBuilder builder)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            services.AddHttpClient();

            return builder;
        }
    }
}
