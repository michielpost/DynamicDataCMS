using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QMS.Storage.CosmosDB.Models;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: HostingStartup(typeof(QMS.Storage.CosmosDB.HostingStartupModule))]
namespace QMS.Storage.CosmosDB
{
    /// <summary>
    /// Configure all services in the QMS.Storage.CosmosDB package
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
            services.Configure<CosmosConfig>(Configuration.GetSection(nameof(CosmosConfig)));

            services.AddTransient<IReadCmsItem, CosmosService>();
            services.AddTransient<IWriteCmsItem, CosmosService>();

        }

        private void ConfigureConfiguration(WebHostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {
            Configuration = configurationBuilder.Build();
        }
    }
}
