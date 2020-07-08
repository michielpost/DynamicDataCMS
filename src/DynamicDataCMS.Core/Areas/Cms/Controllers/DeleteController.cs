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
    public class DeleteController : Controller
    {
        private readonly IReadCmsItem readCmsItemService;
        private readonly IWriteCmsItem writeCmsItemService;
        private readonly JsonSchemaService schemaService;
        private readonly CmsTreeService cmsTreeService;

        public DeleteController(DataProviderWrapperService dataProviderService, JsonSchemaService schemaService, CmsTreeService cmsTreeService)
        {
            this.readCmsItemService = dataProviderService;
            this.writeCmsItemService = dataProviderService;
            this.schemaService = schemaService;
            this.cmsTreeService = cmsTreeService;
        }

        [HttpGet]
        [Route("delete/{cmsType}/{id:guid}/{lang?}")]
        public async Task<IActionResult> Delete([FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang, [FromQuery]string? treeItemSchemaKey, [FromQuery]Guid? treeNodeId)
        {

            if (id == Guid.Empty)
                return new NotFoundResult();

            var cmsMenuItem = schemaService.GetCmsType(cmsType);
            if (cmsMenuItem == null)
                return new NotFoundResult();

            var schemaKey = cmsMenuItem.SchemaKey;
            List<CmsTreeNode> nodes = new List<CmsTreeNode>();

            if (cmsMenuItem.IsTree)
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

            return View("Delete", model);
        }

        [HttpPost]
        [Route("delete/{cmsType}/{id:guid}/{lang?}")]
        public async Task<IActionResult> DeleteConfirm([FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang, [FromQuery]string? treeItemSchemaKey, [FromQuery]Guid? treeNodeId)
        {
            await writeCmsItemService.Delete(cmsType, id, lang, this.User.Identity.Name).ConfigureAwait(false);

            if (treeNodeId.HasValue)
            {
                await cmsTreeService.ClearCmsTreeNode(cmsType, treeNodeId.Value, lang, this.User.Identity.Name).ConfigureAwait(false);
            }

            return RedirectToAction("List", "List", new { cmsType = cmsType });
        }

    }
}
