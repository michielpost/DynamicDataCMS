using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QMS.Core.Controllers
{
    [Area("cms")]
    [Route("[area]/upload")]
    public class UploadController : Controller
    {
        [HttpPost]
        [Route("images")]
        public IActionResult Images([FromForm]IFormFile file)
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

                if (mimeType.StartsWith("image/"))
                {
                    // Only validate files that seem to be images.
                    try
                    {
                        //Image.Load(Configuration.Default, bytes);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return View();
        }

        private static string GetFileName(IFormFile file) => file.ContentDisposition.Split(';')
                                                       .Select(x => x.Trim())
                                                       .Where(x => x.StartsWith("filename="))
                                                       .Select(x => x.Substring(9).Trim('"'))
                                                       .First();
    }
}