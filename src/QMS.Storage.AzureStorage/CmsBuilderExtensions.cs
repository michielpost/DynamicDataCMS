using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using QMS.Storage.AzureStorage.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using QMS.Storage.Interfaces;
using QMS.Core;
using QMS.Models;

namespace QMS.Storage.AzureStorage
{
    /// <summary>
    /// Configure all services in the QMS.Storage.AzureStorage package
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureAzureStorage(this CmsBuilder builder, Func<StorageConfiguration> storageConfigFunc)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            services.Configure<AzureStorageConfig>(Configuration.GetSection(nameof(AzureStorageConfig)));

            services.AddTransient<AzureStorageService>();

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
