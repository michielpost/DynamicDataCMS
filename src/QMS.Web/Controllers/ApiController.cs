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
        [HttpPost]
        [Route("save")]
        public async Task Post([FromBody] dynamic value)
        {
            value.id = "t1324"; //Must be lower case id
            CosmosService cs = new CosmosService();
            await cs.Save(value);
        }

       
    }
}
