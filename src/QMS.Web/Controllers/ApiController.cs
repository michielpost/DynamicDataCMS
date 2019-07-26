using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QMS.Storage.CosmosDB;

namespace QMS.Web.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        CosmosService cs = new CosmosService();

        [HttpPost]
        [Route("save")]
        public async Task Save([FromBody] dynamic value)
        {
            value.id = "t1324"; //Must be lower case id
            await cs.Save(value);
        }

        [HttpGet]
        [Route("load")]
        [Produces("application/json")]
        public async Task<JObject> Load()
        {
            var result = await cs.Load("t1324");
            return result;
        }


    }
}
