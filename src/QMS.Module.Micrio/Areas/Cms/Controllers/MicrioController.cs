using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QMS.Core.Models;
using QMS.Module.Micrio.Models;

namespace QMS.Module.Micrio.Areas.Cms.Controllers
{
    [Area("cms")]
    [Route("[area]/api/micrio")]
    [ApiController]
    public class MicrioController : ControllerBase
    {
        private readonly MicrioConfig micrioConfig;
        private readonly IHttpClientFactory httpClientFactory;

        public MicrioController(IOptions<MicrioConfig> micrioConfig, IHttpClientFactory httpClientFactory)
        {
            this.micrioConfig = micrioConfig.Value;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [Route("add")]
        [Produces("application/json")]
        public async Task<ActionResult<MicrioResponse>> Add([FromBody] string imageUrl)
        {
            var httpClient = httpClientFactory.CreateClient();

            var url = $"https://micr.io/api/external/newImage?apiKey={micrioConfig.ApiKey}&userId={micrioConfig.UserId}&imageUrl={imageUrl}&folderShortId={micrioConfig.FolderShortId}";
            var response = await httpClient.PostAsync(url, new StringContent(string.Empty));

            if(response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<MicrioResponse>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result;
            }

            return new NotFoundResult();
        }
      
    }
}
