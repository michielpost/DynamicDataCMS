using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QMS.Core;
using QMS.Core.Auth;
using QMS.Core.Auth.Models;
using QMS.Core.Models;
using QMS.Core.Services;
using QMS.Core.Services.Extensions;
using QMS.Core.Web.Helpers;
using QMS.Module.Micrio;
using QMS.Storage.AzureStorage;
using QMS.Storage.CosmosDB;
using QMS.Storage.CosmosDB.Models;
using QMS.Storage.Interfaces;
using QMS.Web.Interceptor;

namespace QMS.Web
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
            services.UseQms(Configuration)
                .ConfigureQmsAuth() //Optional if you want user login
                .ConfigureMicrio() //Optional, if you want to have support to upload images to micr.io
                .AddInterceptor<ExampleInterceptor>()
                //.ConfigureCosmosDB(() => new StorageConfiguration() { ReadCmsItems = true })
                .ConfigureAzureStorage(() => new StorageConfiguration() { ReadFiles = true, ReadCmsItems = true });

            services.AddControllersWithViews();

            //Handle routes from the page tree
            services.AddSingleton<PageTreeRoutes>();
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

            //Optional if you want authentication:
            app.UseAuthentication();
            app.UseMiddleware<QmsAuthenticatationMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "cms",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapDynamicControllerRoute<PageTreeRoutes>("{**slug}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
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
