using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QMS.Models;
using QMS.Services;
using QMS.Storage.CosmosDB;
using QMS.Web.Models;

namespace QMS.Web.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly CosmosService cosmosService;
        private readonly JsonSchemaService schemaService;

        public ApiController(CosmosService cosmosService, JsonSchemaService schemaService)
        {
            this.cosmosService = cosmosService;
            this.schemaService = schemaService;
        }

        [HttpPost]
        [Route("save/{cmsType}/{id}/{lang?}")]
        public async Task Save([FromRoute]string cmsType, [FromRoute]string id, [FromBody] CmsDataItem value, [FromRoute]string lang)
        {
            var cmsItem = await cosmosService.Load(cmsType, id);

            if (lang == null)
                cmsItem.AdditionalProperties = value.AdditionalProperties;
            else
                cmsItem.Translations[lang] = value;

            await cosmosService.Save(cmsType, cmsItem);
        }

        [HttpGet]
        [Route("load/{cmsType}/{id}/{lang?}")]
        [Produces("application/json")]
        public async Task<CmsDataItem> Load([FromRoute]string cmsType, [FromRoute]string id, [FromRoute]string lang)
        {
            var cmsItem = await cosmosService.Load(cmsType, id);

            CmsDataItem data = cmsItem;

            if (lang != null)
                data = cmsItem.Translations.GetValueOrDefault(lang);

            return data;
        }


        [HttpGet]
        [Route("list/{cmsType}")]
        [Produces("application/json")]
        public async Task<List<CmsItem>> List([FromRoute]string cmsType)
        {
            var result = await cosmosService.List(cmsType);
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
            var list = await cosmosService.List(cmsType);

            var result = new ExternalEnum
            {
                title = cmsType,
                type = "string",
                @enum = list.Select(x => x.Id).ToList(),
                options = new Options
                {
                     enum_titles = list.Select(x => "TODO: Title: " + x.Id).ToList()
                }
            };
            return result;
        }


    }
}
