using Microsoft.Extensions.Configuration;
using DynamicDataCMS.Storage.AzureStorage.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using DynamicDataCMS.Storage.Interfaces;
using DynamicDataCMS.Core;
using DynamicDataCMS.Core.Models;

namespace DynamicDataCMS.Storage.AzureStorage
{
    /// <summary>
    /// Configure all services in the DynamicDataCMS.Storage.AzureStorage package
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureAzureStorage(this CmsBuilder builder, Func<StorageConfiguration> storageConfigFunc)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            services.Configure<AzureStorageConfig>(Configuration.GetSection(nameof(AzureStorageConfig)));

            services.AddTransient<AzureStorageService>();
            services.AddTransient<AzureTableService>();

            StorageConfiguration storageConfig = storageConfigFunc();

            if(storageConfig.ReadFiles)
                services.AddTransient<IReadFile, CmsFileStorageService>();
            if (storageConfig.WriteFiles)
                services.AddTransient<IWriteFile, CmsFileStorageService>();
            if (storageConfig.ReadCmsItems)
                services.AddTransient<IReadCmsItem, CmsItemStorageService>();
            if (storageConfig.WriteCmsItems)
                services.AddTransient<IWriteCmsItem, CmsItemStorageService>();

            return builder;
        }
    }
}
