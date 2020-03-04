using Microsoft.Extensions.DependencyInjection;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Module.Micrio.Models;
using System.Collections.Generic;

namespace DynamicDataCms.Module.Micrio
{
    /// <summary>
    /// Configure the Micrio Module
    /// </summary>
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder ConfigureMicrio(this CmsBuilder builder)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            services.Configure<MicrioConfig>(Configuration.GetSection(nameof(MicrioConfig)));

            //Add micrio script to CmsConfiguration.EditScripts
            services.PostConfigure<CmsConfiguration>(x =>
            {
                x.EditScripts.Add("/_content/DynamicDataCms.Module.Micrio/qms.micrio.js");
            });

            return builder;

        }
      
    }
}
