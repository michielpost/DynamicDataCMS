using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Core.Services;
using DynamicDataCMS.SampleWeb.Infrastructure;
using DynamicDataCMS.Storage.Interfaces;

namespace DynamicDataCMS.SampleWeb.Controllers
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