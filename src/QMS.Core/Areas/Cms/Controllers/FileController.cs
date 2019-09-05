using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QMS.Services;
using QMS.Storage.Interfaces;

namespace QMS.Core.Controllers
{
    /// <summary>
    /// Handles uploading and downloading files
    /// </summary>
    [Area("cms")]
    [Route("[area]/file")]
    public class FileController : Controller
    {
        private readonly IWriteFile writeFileService;
        private readonly IReadFile readFileService;

        public FileController(DataProviderWrapperService dataProviderService)
        {
            this.writeFileService = dataProviderService;
            this.readFileService = dataProviderService;
        }

        [HttpPost]
        [Route("upload/{cmsType}/{id}/{lang?}")]
        public async Task<JsonResult> Upload([FromForm]IFormFile file, [FromRoute]string cmsType, [FromRoute]string id, [FromRoute]string lang, [FromQuery]string fieldName)
        {
            string fileName = null;
            string mimeType = null;
            byte[] bytes = null;

            if (file != null)
            {
                fileName = GetFileName(file);
                mimeType = file.ContentType.ToLowerInvariant();

                //Get file bytes
                var outputStream = new MemoryStream();
                var output = file.OpenReadStream();
                output.CopyTo(outputStream);
                bytes = outputStream.ToArray();

                //if (mimeType.StartsWith("image/"))
                //{
                //    // Only validate files that seem to be images.
                //    //Image.Load(Configuration.Default, bytes);
                //}

                try
                {
                    var result = await writeFileService.WriteFile(bytes, mimeType, cmsType, id, fieldName, lang).ConfigureAwait(false);

                    return new JsonResult(result);
                }
                catch (Exception e)
                {
                }
            }

            //TODO: some error result
            return new JsonResult("");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Any)]
        [Route("download/{cmsType}/{id}/{fieldName}/{lang?}")]
        public async Task<IActionResult> Download([FromRoute]string cmsType, [FromRoute]string id, [FromRoute]string lang, [FromRoute]string fieldName)
        {
            //TODO: Also return content type?
            var file = await readFileService.ReadFile(cmsType, id, fieldName, lang);

            //TODO: Get document and get proper file name

            return File(file, "application/octet-stream");
        }

        private static string GetFileName(IFormFile file) => file.ContentDisposition.Split(';')
                                                   .Select(x => x.Trim())
                                                   .Where(x => x.StartsWith("filename="))
                                                   .Select(x => x.Substring(9).Trim('"'))
                                                   .First();
    }
}