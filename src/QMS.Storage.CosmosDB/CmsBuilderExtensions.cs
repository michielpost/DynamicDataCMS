using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QMS.Core;
using QMS.Models;
using QMS.Storage.CosmosDB.Models;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QMS.Storage.CosmosDB
{
    /// <summary>
    /// Configure all services in the QMS.Storage.CosmosDB package
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureCosmosDB(this CmsBuilder builder, Func<StorageConfiguration> storageConfigFunc)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            services.Configure<CosmosConfig>(Configuration.GetSection(nameof(CosmosConfig)));

            StorageConfiguration storageConfig = storageConfigFunc();

            if (storageConfig.ReadCmsItems)
                services.AddTransient<IReadCmsItem, CosmosService>();
            if (storageConfig.WriteCmsItems)
                services.AddTransient<IWriteCmsItem, CosmosService>();

            var cosmosConfig = new CosmosConfig();
            Configuration.GetSection(nameof(CosmosConfig)).Bind(cosmosConfig);
            var cosmosConfigOptions = Options.Create<CosmosConfig>(cosmosConfig);

            var cosmosService = new CosmosService(cosmosConfigOptions);
            cosmosService.InitializeContainer();

            return builder;

        }
    }
}
