using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using QMS.Storage.AzureStorage.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using QMS.Storage.Interfaces;

[assembly: HostingStartup(typeof(QMS.Storage.AzureStorage.HostingStartupModule))]
namespace QMS.Storage.AzureStorage
{
    /// <summary>
    /// Configure all services in the QMS.Storage.AzureStorage package
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
            services.Configure<AzureStorageConfig>(Configuration.GetSection(nameof(AzureStorageConfig)));

            services.AddTransient<AzureStorageService>();
            services.AddTransient<IReadFile, CmsFileStorageService>();
            services.AddTransient<IWriteFile, CmsFileStorageService>();
            services.AddTransient<IReadCmsItem, CmsItemStorageService>();
            services.AddTransient<IWriteCmsItem, CmsItemStorageService>();

        }

        private void ConfigureConfiguration(WebHostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {
            Configuration = configurationBuilder.Build();
        }
    }
}
