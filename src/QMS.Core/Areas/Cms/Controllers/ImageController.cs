using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QMS.Services;

namespace QMS.Core.Areas.Cms.Controllers
{
    /// <summary>
    /// Endpoint that returns resized images
    /// </summary>
    [Area("cms")]
    [Route("[area]")]
    public class ImageController : Controller
    {
        private readonly ImageResizeService _imageService;

        public ImageController(ImageResizeService imageService)
        {
            _imageService = imageService;
        }

        //[ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Any)]
        [Route("image/{cmsType}/{id}/{fieldName}/{lang?}")]
        public async Task<IActionResult> GetById([FromRoute]string cmsType, [FromRoute]Guid id, [FromRoute]string? lang, [FromRoute]string fieldName, 
            int? w = null, int? h = null, bool cover = false, int? quality = null)
        {
            try
            {
                var image = await _imageService.GetImageAsync(cmsType, id, fieldName, lang, w, h, cover, quality).ConfigureAwait(false);
                if(image == null)
                    return NotFound();

                return File(image, "image/jpeg");
            }
            catch (ArgumentException error)
            {
                return BadRequest(error.Message);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (Exception error)
            {
                throw error;
            }
        }
    }
}
