using QMS.Core.Services;
using QMS.Core.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Web
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
