﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DynamicDataCMS.Core;
using DynamicDataCMS.Core.Auth;
using DynamicDataCMS.Core.Auth.Models;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Core.Services;
using DynamicDataCMS.Core.Services.Extensions;
using DynamicDataCMS.Core.Web.Helpers;
using DynamicDataCMS.Module.Micrio;
using DynamicDataCMS.Storage.AzureStorage;
using DynamicDataCMS.Storage.CosmosDB;
using DynamicDataCMS.Storage.CosmosDB.Models;
using DynamicDataCMS.Storage.Interfaces;
using DynamicDataCMS.Web.EntityFramework;
using DynamicDataCMS.Web.Interceptor;
using DynamicDataCMS.Storage.EntityFramework;
using DynamicDataCMS.Web.Models;

namespace DynamicDataCMS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CmsDataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.UseDynamicDataCMS(Configuration)
                .ConfigureDynamicDataCmsAuth() //Optional if you want user login
                .ConfigureMicrio() //Optional, if you want to have support to upload images to micr.io
                .AddInterceptor<ExampleInterceptor>()
                //.ConfigureCosmosDB(() => new StorageConfiguration() { ReadCmsItems = true })
                .ConfigureEntityFramework<CmsDataContext, Student>()
                .ConfigureEntityFramework<CmsDataContext, Book>()
                .ConfigureAzureStorage(() => new StorageConfiguration() { ReadFiles = true, ReadCmsItems = true });

            services.AddControllersWithViews();

            //Handle routes from the page tree
            services.AddTransient<PageTreeRoutes>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //Optional if you want authentication:
            app.UseAuthentication();  // Must be after UseRouting()
            app.UseMiddleware<DynamicDataCmsAuthenticatationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "cms",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapDynamicControllerRoute<PageTreeRoutes>("{**slug}");
            });


            //Cosmos
            //using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //{
            //    serviceScope.ServiceProvider.GetService<CosmosService>().InitializeContainer();
            //}

            //Schemas
            Task.Run(async () =>
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    await serviceScope.ServiceProvider.GetService<JsonSchemaService>().InitializeSchemas();

                    //If using auth, insert first test user
                    var dataService = serviceScope.ServiceProvider.GetService<DataProviderWrapperService>();
                    var (_, total) = await dataService.List(CmsUser.DefaultCmsType, null, null);
                    if (total == 0)
                    {
                        var cmsUser = new CmsUser { Email = "admin@admin.com", Password = "admin" };
                        await dataService.Write(cmsUser.ToCmsItem(), CmsUser.DefaultCmsType, Guid.NewGuid(), null, "system");
                    }
                }
            });
        }
    }
}