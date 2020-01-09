using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QMS.Core;
using QMS.Core.Models;
using QMS.Core.Services;
using QMS.SampleWeb.Infrastructure;
using QMS.Storage.AzureStorage;

namespace QMS.SampleWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.UseQms(Configuration)
               .ConfigureAzureStorage(() => new StorageConfiguration() { ReadFiles = true, ReadCmsItems = true });

            services.AddTransient<SampleDataGenerator>();

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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "cms",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapDynamicControllerRoute<PageTreeRoutes>("{**slug}");
            });

            //Initialize Schemas
            Task.Run(async () =>
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    await serviceScope.ServiceProvider.GetService<JsonSchemaService>().InitializeSchemas();
                }
            });
        }
    }
}
