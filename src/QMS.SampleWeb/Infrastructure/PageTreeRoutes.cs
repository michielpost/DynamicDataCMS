using QMS.Core.Services;
using QMS.Core.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.SampleWeb.Infrastructure
{
    public class PageTreeRoutes : CmsTreeRoutes
    {
        public static string PageTreeType => "pagetree";

        public override string CmsTreeType => PageTreeType;

        public PageTreeRoutes(CmsTreeService cmsTreeService, DataProviderWrapperService dataProviderService)
            : base(cmsTreeService, dataProviderService)
        {

        }
    }
}
