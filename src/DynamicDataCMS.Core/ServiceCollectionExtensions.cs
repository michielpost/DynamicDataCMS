using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataCms.Core
{
    public static class IWebHostBuilderExtensions
    {
        public static CmsBuilder UseQms(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = new CmsBuilder(services, configuration)
                .ConfigureCoreCms()
                .ConfigureQmsServices();

            return builder;
        }
    }
}
