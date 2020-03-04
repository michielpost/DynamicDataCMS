using Microsoft.Extensions.DependencyInjection;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Module.Micrio.Models;
using System.Collections.Generic;

namespace DynamicDataCMS.Module.Micrio
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
                x.EditScripts.Add("/_content/DynamicDataCMS.Module.Micrio/qms.micrio.js");
            });

            return builder;

        }
      
    }
}
