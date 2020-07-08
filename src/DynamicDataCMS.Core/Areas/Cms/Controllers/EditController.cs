using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Core.Services;
using DynamicDataCMS.Storage.Interfaces;
using System.Net.Http;
using NJsonSchema;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DynamicDataCMS.Core.Areas.Cms.Controllers
{
    [Area("cms")]
    [Route("[area]")]
    public class EditController : Controller
    {
        private readonly IReadCmsItem readCmsItemService;
        private readonly IWriteCmsItem writeCmsItemService;
        private readonly JsonSchemaService schemaService;
        private readonly CmsTreeService cmsTreeService;
        private readonly IHttpClientFactory httpClientFactory;

        public EditController(DataProviderWrapperService dataProviderService, JsonSchemaService schemaService, IHttpClientFactory clientFactory, CmsTreeService cmsTreeService)
        {
            this.readCmsItemService = dataProviderService;
            this.writeCmsItemService = dataProviderService;
            this.schemaService = schemaService;
            this.cmsTreeService = cmsTreeService;
            this.httpClientFactory = clientFactory;
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

            return RedirectToAction("Edit", "JsonEditor", this.Request.RouteValues);
        }

        [Route("edittree/{cmsTreeType}/{**slug}")]
        [HttpGet]
        public async Task<IActionResult> EditTree([FromRoute]string cmsTreeType, [FromRoute]string slug, [FromQuery]string? treeItemSchemaKey, [FromQuery]string? lang)
        {
            slug ??= string.Empty;
            slug = slug.TrimStart('/');
            slug = "/" + slug;
           

            var cmsMenuItem = schemaService.GetCmsType(cmsTreeType);
            if (cmsMenuItem == null || !cmsMenuItem.SchemaKeys.Any())
                return new NotFoundResult();

            CmsTreeNode? cmsTreeItem = await cmsTreeService.GetCmsTreeNode(cmsTreeType, slug, lang).ConfigureAwait(false);
            if(cmsTreeItem == null)
                cmsTreeItem = await cmsTreeService.CreateOrUpdateCmsTreeNodeForSlug(cmsTreeType, slug, new CmsTreeNode() { CmsItemId = Guid.NewGuid() }, lang, this.User.Identity.Name);

            if (string.IsNullOrEmpty(cmsTreeItem.CmsItemType) && string.IsNullOrEmpty(treeItemSchemaKey))
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

            var httpClient = httpClientFactory.CreateClient();
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
       
    }
}
