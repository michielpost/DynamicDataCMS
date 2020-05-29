using Microsoft.Extensions.DependencyInjection;
using System;
using DynamicDataCMS.Storage.Interfaces;
using DynamicDataCMS.Core;
using DynamicDataCMS.Core.Models;

namespace DynamicDataCMS.Storage.SiaSkynet
{
    /// <summary>
    /// Configure all services in the DynamicDataCMS.Storage.SiaSkynet package
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureSiaSkynet(this CmsBuilder builder)
        {
            return builder.ConfigureSiaSkynet(() => new StorageConfiguration { WriteFiles = true, ReadFiles = true });
        }

        public static CmsBuilder ConfigureSiaSkynet(this CmsBuilder builder, Func<StorageConfiguration> storageConfigFunc)
        {
            var services = builder.Services;

            StorageConfiguration storageConfig = storageConfigFunc();

            if(storageConfig.ReadFiles)
                services.AddTransient<IReadFile, CmsFileStorageService>();
            if (storageConfig.WriteFiles)
                services.AddTransient<IWriteFile, CmsFileStorageService>();

            return builder;
        }
    }
}
