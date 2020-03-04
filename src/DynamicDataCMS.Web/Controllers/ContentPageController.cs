using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DynamicDataCms.Core.Models;

namespace DynamicDataCms.Web.Controllers
{
    public class ContentPageController : Controller
    {
        public IActionResult Index(Guid cmsItemId)
        {
            return View();
        }
    }
}