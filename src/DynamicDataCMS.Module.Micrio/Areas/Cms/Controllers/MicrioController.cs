using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DynamicDataCMS.Core.Models;
using DynamicDataCMS.Module.Micrio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DynamicDataCMS.Module.Micrio.Areas.Cms.Controllers
{
    [Area("cms")]
    [Route("[area]/micrio")]
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
        public async Task<ActionResult<MicrioResponse?>> Add([FromBody] string imageId)
        {
            if (!this.micrioConfig.IsValid())
                throw new Exception("Micrio config is not valid.");

            var httpClient = httpClientFactory.CreateClient();

            string imageUrl = Url.Action("NoImage", "Image", new { Area = "Cms"}, this.Request.Scheme) + "/" + imageId;

            var url = $"https://micr.io/api/external/newImage?apiKey={micrioConfig.ApiKey}&userId={micrioConfig.UserId}&imageUrl={imageUrl}&folderShortId={micrioConfig.FolderShortId}";
            var response = await httpClient.PostAsync(url, new StringContent(string.Empty));

            if (response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<MicrioResponse>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.Content != null)
                    response.Content.Dispose();

                throw new Exception("Error submitting to Micr.io: " + content);
            }
            
        }
      
    }
}
