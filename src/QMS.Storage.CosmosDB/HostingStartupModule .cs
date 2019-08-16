using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QMS.Storage.CosmosDB.Models;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: HostingStartup(typeof(QMS.Storage.CosmosDB.HostingStartupModule))]
namespace QMS.Storage.CosmosDB
{
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

            services.AddTransient<CosmosService>();

        }

        private void ConfigureConfiguration(WebHostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {
            Configuration = configurationBuilder.Build();
        }
    }
}
