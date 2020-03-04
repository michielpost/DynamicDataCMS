using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataCMS.Core.Models
{
    public class CmsBuilder
    {
        public IServiceCollection Services { get; set; }
        public IConfiguration Configuration { get; set; }

        public CmsBuilder(IServiceCollection services, IConfiguration configuration)
        {
            this.Services = services;
            this.Configuration = configuration;
        }
    }
}
