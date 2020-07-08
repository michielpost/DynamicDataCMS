using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Core.Services;
using DynamicDataCMS.Storage.Interfaces;
using System.Net.Http;
using NJsonSchema;
using System.Text.Json;
using DynamicDataCMS.Editors.JsonEditor.Models;

namespace DynamicDataCMS.Editors.JsonEditor.Areas.Cms.Controllers
{
    [Area("cms")]
    [Route("[area]")]
    public class JsonEditorController : Controller
    {
        private readonly IReadCmsItem readCmsItemService;
        private readonly JsonSchemaService schemaService;
        private readonly CmsTreeService cmsTreeService;
        private readonly IHttpClientFactory httpClientFactory;

        public JsonEditorController(DataProviderWrapperService dataProviderService, JsonSchemaService schemaService, IHttpClientFactory clientFactory, CmsTreeService cmsTreeService)
        {
            this.readCmsItemService = dataProviderService;
            this.schemaService = schemaService;
            this.cmsTreeService = cmsTreeService;
            this.httpClientFactory = clientFactory;
        }

        [Route("jsoneditor/edit/{cmsType}/{id}/{lang?}")]
        [HttpGet]
        public async Task<IActionResult> JsonEditor([FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang, [FromQuery]string? treeItemSchemaKey, [FromQuery]Guid? treeNodeId)
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
            return View(nameof(JsonEditor), model);

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
            return View(nameof(JsonEditor), model);
        }


    }
}
