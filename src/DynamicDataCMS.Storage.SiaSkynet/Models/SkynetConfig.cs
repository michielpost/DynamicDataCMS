using DynamicDataCMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicDataCMS.Storage.SiaSkynet.Models
{
    public class SkynetConfig
    {
        public string? BaseUrl { get; set; }

        public string? Secret { get; set; }

        /// <summary>
        /// Types that should not be saved by this provider
        /// </summary>
        public List<CmsType> ExcludedTypes { get; set; } = new List<CmsType>();
    }
}
