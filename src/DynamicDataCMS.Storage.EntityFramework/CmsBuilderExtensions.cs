using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Interfaces;
using System;

namespace DynamicDataCMS.Storage.EntityFramework
{
    /// <summary>
    /// Configure all services in the DynamicDataCMS.Storage.EntityFramework package
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
