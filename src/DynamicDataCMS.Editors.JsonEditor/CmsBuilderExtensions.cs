using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DynamicDataCMS.Core;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Core
{
    public static class CmsBuilderExtensions
    {
        public static CmsBuilder UseJsonEditor(this CmsBuilder builder)
        {
            var Configuration = builder.Configuration;
            var services = builder.Services;

            return builder;
        }
    }
}
