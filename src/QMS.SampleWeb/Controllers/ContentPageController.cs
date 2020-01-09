using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QMS.Core.Models;
using QMS.Core.Services;
using QMS.SampleWeb.Infrastructure;
using QMS.Storage.Interfaces;

namespace QMS.SampleWeb.Controllers
{
    public class ContentPageController : Controller
    {
        private readonly IReadCmsItem dataProviderService;

        public ContentPageController(DataProviderWrapperService dataProviderService)
        {
            this.dataProviderService = dataProviderService;
        }
        public async Task<IActionResult> Index(Guid cmsItemId)
        {
            var item = await dataProviderService.Read<CmsItem>(PageTreeRoutes.PageTreeType, cmsItemId, null);
            return View(item);
        }
    }
}