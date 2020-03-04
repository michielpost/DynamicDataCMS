using DynamicDataCMS.Core.Services;
using DynamicDataCMS.Core.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicDataCMS.Web
{
    public class PageTreeRoutes : CmsTreeRoutes
    {
        public override string CmsTreeType => "pagetree";

        public PageTreeRoutes(CmsTreeService cmsTreeService, DataProviderWrapperService dataProviderService)
            : base(cmsTreeService, dataProviderService)
        {

        }
    }
}
