using Microsoft.Extensions.DependencyInjection;
using System;
using DynamicDataCMS.Storage.Interfaces;
using DynamicDataCMS.Core;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Ipfs.Models;

namespace DynamicDataCMS.Storage.Ipfs
{
    /// <summary>
    /// Configure all services in the DynamicDataCMS.Storage.Ipfs package
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureIpfs(this CmsBuilder builder)
        {
            return builder.ConfigureIpfs(() => new StorageConfiguration { WriteFiles = true, ReadFiles = true });
        }

        public static CmsBuilder ConfigureIpfs(this CmsBuilder builder, Func<StorageConfiguration> storageConfigFunc)
        {
            var services = builder.Services;
            var Configuration = builder.Configuration;

            services.Configure<IpfsConfig>(Configuration.GetSection(nameof(IpfsConfig)));

            StorageConfiguration storageConfig = storageConfigFunc();

            if(storageConfig.ReadFiles)
                services.AddTransient<IReadFile, CmsFileStorageService>();
            if (storageConfig.WriteFiles)
                services.AddTransient<IWriteFile, CmsFileStorageService>();

            return builder;
        }
    }
}
