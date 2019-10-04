using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QMS.Models;
using QMS.Services;
using QMS.Services.Models;
using QMS.Core.Models;
using QMS.Storage.Interfaces;
using NJsonSchema;

namespace QMS.Core.Controllers
{
    [Area("cms")]
    [Route("[area]/api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IReadCmsItem readCmsItemService;
        private readonly IWriteCmsItem writeCmsItemService;
        private readonly JsonSchemaService schemaService;

        public ApiController(DataProviderWrapperService dataProviderService, JsonSchemaService schemaService)
        {
            this.readCmsItemService = dataProviderService;
            this.writeCmsItemService = dataProviderService;
            this.schemaService = schemaService;
        }

        [HttpPost]
        [Route("save/{cmsType}/{id}/{lang?}")]
        [Produces("application/json")]
        public async Task<ActionResult> Save([FromRoute]string cmsType, [FromRoute]string id, [FromBody] CmsItemPostModel value, [FromRoute]string? lang)
        {
            CmsItem item = new CmsItem
            {
                AdditionalProperties = value.AdditionalProperties,
                CmsType = cmsType,
                Id = id
            };
            await writeCmsItemService.Write(item, cmsType, id, lang).ConfigureAwait(false);

            return new OkObjectResult(item);
        }

        [HttpGet]
        [Route("load/{cmsType}/{id}/{lang?}")]
        [Produces("application/json")]
        public async Task<CmsItem?> Load([FromRoute]string cmsType, [FromRoute]string id, [FromRoute]string? lang)
        {
            var cmsItem = await readCmsItemService.Read(cmsType, id, lang).ConfigureAwait(false);

            return cmsItem;
        }


        [HttpGet]
        [Route("list/{cmsType}")]
        [Produces("application/json")]
        public async Task<IReadOnlyList<CmsItem>> List([FromRoute]string cmsType, [FromQuery]string? sortField, [FromQuery]string? sortOrder)
        {
            var result = await readCmsItemService.List(cmsType, sortField, sortOrder).ConfigureAwait(false);
            return result;
        }


        /// <summary>
        /// Used to make references to other entities by providing a dynamic enum json schema
        /// </summary>
        /// <param name="cmsType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("enum/{cmsType}")]
        [Produces("application/json")]
        public async Task<ExternalEnum> Enum([FromRoute]string cmsType)
        {
            var schema = schemaService.GetSchema(cmsType);
            var list = await readCmsItemService.List(cmsType, null, null).ConfigureAwait(false);

            var result = new ExternalEnum
            {
                title = cmsType,
                type = "string",
                @enum = list.Select(x => x.Id).ToList(),
                options = new Options
                {
                     enum_titles = list.Select(x => GetDisplayTitle(x, schema)).ToList()
                }
            };
            return result;
        }

        [HttpGet]
        [Route("schema/{assemblyName}/{typeName}")]
        [Produces("application/json")]
        public ActionResult Schema([FromRoute]string assemblyName, [FromRoute]string typeName)
        {
            Type? type = Type.GetType($"{typeName}, {assemblyName}");
            if (type == null)
                return new NotFoundResult();

            var schema = JsonSchema.FromType(type, new NJsonSchema.Generation.JsonSchemaGeneratorSettings()
            {
                //AllowReferencesWithProperties = true,
                //AlwaysAllowAdditionalObjectProperties = true,
                DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull,
                GenerateAbstractProperties = true,
                FlattenInheritanceHierarchy = true,
            });

            return Content(schema.ToJson());
        }

        private string GetDisplayTitle(CmsItem x, SchemaLocation schema)
        {
            List<string> titles = new List<string>();

            foreach(var prop in schema.ListViewProperties)
            {
                titles.Add(x.AdditionalProperties.FirstOrDefault(p => p.Key == prop.Key).Value.ToString());
            }

            string result = string.Join(" ", titles);
            if (string.IsNullOrWhiteSpace(result))
                result = x.Id;

            return result;
        }
    }
}
