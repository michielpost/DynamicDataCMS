using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DynamicDataCms.Core;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Storage.Interfaces;

namespace DynamicDataCms.Core.Services
{
    /// <summary>
    /// Configure the CMS
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureQmsServices(this CmsBuilder builder, string cmsConfigFileName = "CmsConfiguration.json")
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            var cmsConfig = new ConfigurationBuilder()
            //.SetBasePath(env.ContentRootPath)
            .AddJsonFile(cmsConfigFileName)
            .Build();

            //CmsConfiguration
            //services.Configure<CmsConfigLocation>(Configuration.GetSection(nameof(CmsConfigLocation)));
            services.Configure<CmsConfiguration>(cmsConfig);

            services.AddTransient<JsonSchemaService>();
            services.AddTransient<CmsTreeService>();
            services.AddTransient<ImageResizeService>();
            services.AddTransient<DataProviderWrapperService>();

            return builder;

        }

        public static CmsBuilder AddInterceptor<T>(this CmsBuilder builder) where T : class, IWriteCmsItemInterceptor
        {
            builder.Services.AddSingleton<IWriteCmsItemInterceptor, T>();

            return builder;
        }
    }
}
