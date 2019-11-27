using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QMS.Core.Models;
using QMS.Core.Services;
using QMS.Storage.Interfaces;
using System.Net.Http;
using NJsonSchema;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace QMS.Core.Areas.Cms.Controllers
{
    [Area("cms")]
    [Route("[area]")]
    public class HomeController : Controller
    {
        private readonly IReadCmsItem readCmsItemService;
        private readonly IWriteCmsItem writeCmsItemService;
        private readonly JsonSchemaService schemaService;
        private readonly IHttpClientFactory httpClientFactory;

        public HomeController(DataProviderWrapperService dataProviderService, JsonSchemaService schemaService, IHttpClientFactory clientFactory)
        {
            this.readCmsItemService = dataProviderService;
            this.writeCmsItemService = dataProviderService;
            this.schemaService = schemaService;
            this.httpClientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("list/{cmsType}")]
        [HttpGet]
        public async Task<IActionResult> List([FromRoute]string cmsType, [FromQuery]string? sortField, [FromQuery]string? sortOrder, [FromQuery]int pageIndex, [FromQuery]string? q)
        {
            var schema = schemaService.GetSchema(cmsType);
            int pageSize = schema.PageSize;
            if (pageSize <= 0)
                pageSize = 20;

            var (results, total) = await readCmsItemService.List(cmsType, sortField, sortOrder, pageSize: pageSize, pageIndex, q).ConfigureAwait(false);

            ViewBag.SortField = sortField;
            ViewBag.SortOrder = sortOrder;

            if (schema.IsSingleton)
            {
                if (results.Any())
                    return RedirectToAction("Edit", new { cmsType = cmsType, id = results.First().Id });
                else
                    return RedirectToAction("Create", new { cmsType = cmsType });
            }

            var model = new ListViewModel
            {
                CmsType = cmsType,
                Schema = schema,
                Items = results,
                TotalPages = total,
                CurrentPage = pageIndex + 1,
                PageSize = pageSize
            };
            return View(model);
        }

        [Route("edit/{cmsType}/{id}/{lang?}")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang)
        {
            if (id == Guid.Empty)
                return new NotFoundResult();

            var schema = schemaService.GetSchema(cmsType);
            var data = await readCmsItemService.Read<CmsItem>(cmsType, id, lang).ConfigureAwait(false);

            var model = new EditViewModel
            {
                CmsType = cmsType,
                Id = id,
                SchemaLocation = schema,
                CmsConfiguration = schemaService.GetCmsConfiguration(),
                Language = lang,
                Data = data
            };
            return View(model);
        }

        /// <summary>
        /// Dynamic Edit endpoint, accepts any json url and generates an editor
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [Route("edit/_dynamic")]
        [HttpGet]
        public async Task<IActionResult> EditDynamic([FromQuery]string url)
        {
            if (string.IsNullOrEmpty(url))
                return new NotFoundResult();

            var httpClient = httpClientFactory.CreateClient();
            var json = await httpClient.GetStringAsync(url).ConfigureAwait(false);

            var jsonSchema = JsonSchema.FromSampleJson(json);

            var schema = new SchemaLocation()
            {
                Key = "_dynamic",
                Name = "Dynamic",
                Schema = jsonSchema.ToJson()
            };

            var model = new EditViewModel
            {
                CmsType = "_dynamic",
                Id = Guid.NewGuid(),
                SchemaLocation = schema,
                CmsConfiguration = schemaService.GetCmsConfiguration(),
                Data = JsonSerializer.Deserialize<CmsItem>(json)
            };
            return View(nameof(Edit), model);
        }

        [Route("create/{cmsType}")]
        [HttpGet]
        public IActionResult Create([FromRoute]string cmsType)
        {
            return RedirectToAction("Edit", new { cmsType = cmsType, id = Guid.NewGuid() });
        }

        [HttpGet]
        [Route("delete/{cmsType}/{id:guid}/{lang?}")]
        public async Task<IActionResult> Delete([FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang)
        {
            var schema = schemaService.GetSchema(cmsType);
            var data = await readCmsItemService.Read<CmsItem>(cmsType, id, lang).ConfigureAwait(false);

            var model = new EditViewModel
            {
                CmsType = cmsType,
                Id = id,
                SchemaLocation = schema,
                Data = data
            };
            return View("Delete", model);
        }

        [HttpPost]
        [Route("delete/{cmsType}/{id:guid}/{lang?}")]
        public async Task<IActionResult> DeleteConfirm([FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang)
        {
            await writeCmsItemService.Delete(cmsType, id, lang, this.User.Identity.Name).ConfigureAwait(false);

            return RedirectToAction("List", new { cmsType = cmsType });
        }


        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
