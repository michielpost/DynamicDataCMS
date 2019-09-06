using Microsoft.Extensions.DependencyInjection;
using QMS.Core;
using QMS.Services.Models;

namespace QMS.Services
{
    /// <summary>
    /// Configure all services in the QMS.Storage.CosmosDB package
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureQmsServices(this CmsBuilder builder)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            //CmsConfiguration
            services.Configure<CmsConfigLocation>(Configuration.GetSection(nameof(CmsConfigLocation)));

            services.AddTransient<JsonSchemaService>();
            services.AddTransient<ImageResizeService>();
            services.AddTransient<DataProviderWrapperService>();

            return builder;

        }
      
    }
}
