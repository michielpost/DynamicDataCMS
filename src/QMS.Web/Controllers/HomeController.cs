using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QMS.Services;
using QMS.Storage.CosmosDB;
using QMS.Web.Models;

namespace QMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly CosmosService cosmosService;
        private readonly JsonSchemaService schemaService;

        public HomeController(CosmosService cosmosService, JsonSchemaService schemaService)
        {
            this.cosmosService = cosmosService;
            this.schemaService = schemaService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var all = await schemaService.GetSchemas();

            return View(all);
        }

        [Route("list/{cmsType}")]
        public async Task<IActionResult> List([FromRoute]string cmsType)
        {
            var result = await cosmosService.List(cmsType);
            var schema = await schemaService.GetSchema(cmsType);

            if(schema.IsSingleton)
            {
                if (result.Any())
                    return RedirectToAction("Edit", new { cmsType = cmsType, id = result.First().id });
                else
                    return RedirectToAction("Create", new { cmsType = cmsType });
            }

            var model = new ListViewModel
            {
                CmsType = cmsType,
                Schema = schema,
                Items = result
            };
            return View(model);
        }

        [Route("edit/{cmsType}/{id}")]
        public async Task<IActionResult> Edit([FromRoute]string cmsType, [FromRoute]string id)
        {
            if (string.IsNullOrEmpty(id))
                return new NotFoundResult();

            var schema = await schemaService.GetSchema(cmsType);
            var data = await cosmosService.Load(cmsType, id);

            var model = new EditViewModel
            {
                 CmsType = cmsType,
                 Id = id,
                 SchemaLocation = schema,
                 Data = JsonConvert.SerializeObject(data)
            };
            return View(model);
        }

        [Route("create/{cmsType}")]
        public async Task<IActionResult> Create([FromRoute]string cmsType)
        {
            var schema = await schemaService.GetSchema(cmsType);

            var model = new EditViewModel
            {
                CmsType = cmsType,
                Id = Guid.NewGuid().ToString(),
                SchemaLocation = schema,
            };
            return View("Edit", model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
