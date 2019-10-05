using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QMS.Core;
using QMS.Models;

namespace QMS.Services
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
            services.AddTransient<ImageResizeService>();
            services.AddTransient<DataProviderWrapperService>();

            return builder;

        }
      
    }
}
