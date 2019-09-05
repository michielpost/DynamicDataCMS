using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: HostingStartup(typeof(QMS.Core.HostingStartupModule))]
namespace QMS.Core
{
    /// <summary>
    /// Configure all services for QMS.Core
    /// </summary>
    public class HostingStartupModule : IHostingStartup
    {
        private IConfiguration Configuration { get; set; }

        public void Configure(IWebHostBuilder builder)
        {

            builder.ConfigureAppConfiguration(ConfigureConfiguration);
            builder.ConfigureServices(ConfigureServices);
        }

        private void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddTransient<IPostConfigureOptions<StaticFileOptions>, CmsConfigureOptions>();
        }

        private void ConfigureConfiguration(WebHostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {
            Configuration = configurationBuilder.Build();
        }
    }
}
