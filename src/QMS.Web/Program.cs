using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QMS.Core;
using QMS.Storage.AzureStorage;
using QMS.Storage.CosmosDB;

namespace QMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseQms(new CmsBuilder()
                            .AddCoreCms()
                            .AddAzureStorage()
                            .AddCosmosDB())
                .UseStartup<Startup>();
    }
}
