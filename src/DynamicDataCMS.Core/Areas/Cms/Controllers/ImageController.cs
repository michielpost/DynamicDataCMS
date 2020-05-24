using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DynamicDataCMS.Core.Services;
using SixLabors.ImageSharp;

namespace DynamicDataCMS.Core.Areas.Cms.Controllers
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

        [Route("image")]
        public IActionResult NoImage()
        {
            return Redirect("~/_content/DynamicDataCMS.Core/transparant.png");
        }

        //[ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Any)]
        [Route("image/{**fileName}")]
        public async Task<IActionResult> GetById([FromRoute]string fileName, 
            int? w = null, int? h = null, bool cover = false, int? quality = null)
        {
            try
            {
                var image = await _imageService.GetImageAsync(fileName, w, h, cover, quality).ConfigureAwait(false);
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
