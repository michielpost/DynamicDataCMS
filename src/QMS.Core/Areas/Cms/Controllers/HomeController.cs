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

namespace QMS.Core.Controllers
{
    [Area("cms")]
    [Route("[area]")]
    public class HomeController : Controller
    {
        private readonly IReadCmsItem readCmsItemService;
        private readonly IWriteCmsItem writeCmsItemService;
        private readonly JsonSchemaService schemaService;
        private readonly CmsTreeService cmsTreeService;

        public HomeController(DataProviderWrapperService dataProviderService, JsonSchemaService schemaService, CmsTreeService cmsTreeService)
        {
            this.readCmsItemService = dataProviderService;
            this.writeCmsItemService = dataProviderService;
            this.schemaService = schemaService;
            this.cmsTreeService = cmsTreeService;
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
            var cmsMenuItem = schemaService.GetCmsType(cmsType);
            if (cmsMenuItem == null)
                return new NotFoundResult();

            if (cmsMenuItem.IsTree)
                return await ListTree(cmsType, null);

            if (cmsMenuItem.SchemaKey == null)
                return new NotFoundResult();

            var schema = schemaService.GetSchema(cmsMenuItem.SchemaKey);
            int pageSize = cmsMenuItem.PageSize;
            if (pageSize <= 0)
                pageSize = 20;

            var (results, total) = await readCmsItemService.List(cmsType, sortField, sortOrder, pageSize: pageSize, pageIndex, q).ConfigureAwait(false);

            ViewBag.SortField = sortField;
            ViewBag.SortOrder = sortOrder;

            if (cmsMenuItem.IsSingleton)
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
                MenuCmsItem = cmsMenuItem,
                Items = results,
                TotalPages = total,
                CurrentPage = pageIndex + 1,
                PageSize = pageSize
            };
            return View(model);
        }

        public async Task<IActionResult> ListTree(string cmsType, string? lang)
        {
            var cmsMenuItem = schemaService.GetCmsType(cmsType);
            if (cmsMenuItem == null || !cmsMenuItem.IsTree)
                return new NotFoundResult();

            var (results, total) = await readCmsItemService.List(cmsType, null, null).ConfigureAwait(false);
            var treeItem = await cmsTreeService.GetCmsTreeItem(cmsType, lang);

            if (treeItem == null)
                return new NotFoundResult();

            var model = new ListTreeViewModel
            {
                CmsType = cmsType,
                MenuCmsItem = cmsMenuItem,
                CmsTreeItem = treeItem,
                Items = results,
            };

            return View("ListTree", model);
        }

        [Route("edit/{cmsType}/{id}/{lang?}")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang, [FromQuery]string? treeItemSchemaKey, [FromQuery]Guid? treeNodeId)
        {
            if (id == Guid.Empty)
                return new NotFoundResult();

            var cmsMenuItem = schemaService.GetCmsType(cmsType);
            if(cmsMenuItem == null)
                return new NotFoundResult();

            var schemaKey = cmsMenuItem.SchemaKey;
            List<CmsTreeNode> nodes = new List<CmsTreeNode>();

            if(cmsMenuItem.IsTree)
            {
                nodes = await cmsTreeService.GetCmsTreeNodesForCmsItemId(cmsType, id, lang);
                if (nodes.Any())
                {
                    schemaKey = nodes.First().CmsItemType;
                }
                else
                {
                    schemaKey = treeItemSchemaKey;
                    if (treeNodeId.HasValue)
                        nodes = await cmsTreeService.GetCmsTreeNodesForNodeId(cmsType, treeNodeId.Value, lang);
                }


                if (schemaKey == null)
                {
                    //TODO: Forward to pick a schema
                }
            }

            if (schemaKey == null)
                return new NotFoundResult();

            var schema = schemaService.GetSchema(schemaKey);
            if (schema == null)
                return new NotFoundResult();

            var data = await readCmsItemService.Read<CmsItem>(cmsType, id, lang).ConfigureAwait(false);

            var model = new EditViewModel
            {
                CmsType = cmsType,
                Id = id,
                SchemaLocation = schema,
                MenuCmsItem = cmsMenuItem,
                CmsConfiguration = schemaService.GetCmsConfiguration(),
                Language = lang,
                Data = data,
                Nodes = nodes,
                TreeItemSchemaKey = treeItemSchemaKey,
                TreeNodeId = treeNodeId
            };
            return View("Edit", model);

        }

        [Route("edittree/{cmsTreeType}/{**slug}")]
        [HttpGet]
        public async Task<IActionResult> EditTree([FromRoute]string cmsTreeType, [FromRoute]string slug, [FromQuery]string? treeItemSchemaKey, [FromQuery]string? lang)
        {
            slug ??= string.Empty;
            slug = "/" + slug;

            var cmsMenuItem = schemaService.GetCmsType(cmsTreeType);
            if (cmsMenuItem == null || !cmsMenuItem.SchemaKeys.Any())
                return new NotFoundResult();

            CmsTreeNode? cmsTreeItem = await cmsTreeService.GetCmsTreeNode(cmsTreeType, slug, lang).ConfigureAwait(false);

            if (cmsTreeItem == null || string.IsNullOrEmpty(cmsTreeItem.CmsItemType))
            {
                if (string.IsNullOrEmpty(treeItemSchemaKey))
                {
                    EditTreeViewModel vm = new EditTreeViewModel
                    {
                        MenuCmsItem = cmsMenuItem,
                        CmsType = cmsTreeType,
                        Language = lang,
                        TreeItemSchemaKey = treeItemSchemaKey,
                        TreeNodeId = cmsTreeItem?.NodeId
                    };

                    return View(vm);
                }

                cmsTreeItem = new CmsTreeNode { CmsItemId = Guid.NewGuid(), CmsItemType = treeItemSchemaKey };
            }

            Guid? id = cmsTreeItem.CmsItemId;
            if (!id.HasValue)
                id = Guid.NewGuid();

            return RedirectToAction("Edit", new { cmsType = cmsTreeType, id = id, lang = lang, treeItemSchemaKey = treeItemSchemaKey, treeNodeId = cmsTreeItem.NodeId });
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

            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(url).ConfigureAwait(false);

            var jsonSchema = JsonSchema.FromSampleJson(json);

            var schema = new SchemaLocation()
            {
                Key = "_dynamic",
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
            var cmsMenuItem = schemaService.GetCmsType(cmsType);
            if (cmsMenuItem == null || cmsMenuItem.SchemaKey == null)
                return new NotFoundResult();

            var schema = schemaService.GetSchema(cmsMenuItem.SchemaKey);
            var data = await readCmsItemService.Read<CmsItem>(cmsType, id, lang).ConfigureAwait(false);

            var model = new EditViewModel
            {
                CmsType = cmsType,
                MenuCmsItem = cmsMenuItem,
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
