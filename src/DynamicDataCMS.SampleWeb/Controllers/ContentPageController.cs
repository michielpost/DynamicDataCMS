using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DynamicDataCms.Core.Models;
using DynamicDataCms.Core.Services;
using DynamicDataCms.SampleWeb.Infrastructure;
using DynamicDataCms.Storage.Interfaces;

namespace DynamicDataCms.SampleWeb.Controllers
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