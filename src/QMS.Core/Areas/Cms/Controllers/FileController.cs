using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QMS.Models;
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
        private readonly ILogger<FileController> logger;

        public FileController(DataProviderWrapperService dataProviderService, ILoggerFactory loggerFactory)
        {
            this.writeFileService = dataProviderService;
            this.readFileService = dataProviderService;
            this.logger = loggerFactory.CreateLogger<FileController>();
        }

        [HttpPost]
        [Route("upload/{cmsType}/{id}/{lang?}")]
        public async Task<JsonResult> Upload([FromForm]IFormFile file, [FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang, [FromQuery]string fieldName)
        {
            if (file != null)
            {
                string fileName = GetFileName(file);
                string mimeType = file.ContentType.ToLowerInvariant();

                //Get file bytes
                var outputStream = new MemoryStream();
                var output = file.OpenReadStream();
                output.CopyTo(outputStream);
                byte[] bytes = outputStream.ToArray();

                //if (mimeType.StartsWith("image/"))
                //{
                //    // Only validate files that seem to be images.
                //    //Image.Load(Configuration.Default, bytes);
                //}

                CmsFile cmsFile = new CmsFile
                {
                    Bytes = bytes,
                    ContentType = mimeType
                };

                try
                {
                    var result = await writeFileService.WriteFile(cmsFile, cmsType, id, fieldName, lang).ConfigureAwait(false);

                    return new JsonResult(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error writing file.");
                    throw;
                }
            }

            return new JsonResult("No file submitted");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Any)]
        [Route("download/{cmsType}/{id}/{fieldName}/{lang?}")]
        public async Task<IActionResult> Download([FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang, [FromRoute]string fieldName)
        {
            var file = await readFileService.ReadFile(cmsType, id, fieldName, lang).ConfigureAwait(false);
            if(file == null)
                return NotFound();

            //TODO: Get document and get proper file name

            return File(file.Bytes, file.ContentType);
        }

        private static string GetFileName(IFormFile file) => file.ContentDisposition.Split(';')
                                                   .Select(x => x.Trim())
                                                   .Where(x => x.StartsWith("filename="))
                                                   .Select(x => x.Substring(9).Trim('"'))
                                                   .First();
    }
}