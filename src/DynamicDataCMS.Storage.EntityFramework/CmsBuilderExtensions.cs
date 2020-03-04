using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Storage.Interfaces;
using System;

namespace DynamicDataCms.Storage.EntityFramework
{
    /// <summary>
    /// Configure all services in the DynamicDataCms.Storage.EntityFramework package
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureEntityFramework<Context, Model>(this CmsBuilder builder) where Context : DbContext where Model : class
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;
            
            services.AddTransient<IReadCmsItem, DatabaseService<Context, Model>>();
            services.AddTransient<IWriteCmsItem, DatabaseService<Context, Model>>();

            return builder;

        }
    }
}
