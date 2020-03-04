using DynamicDataCMS.Storage.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataCMS.Core.Services
{
    /// <summary>
    /// Handles all the image resizing
    /// </summary>
    public class ImageResizeService
    {
        private readonly int compressionQuality = 75;
        private readonly IReadFile readFileService;

        public ImageResizeService(DataProviderWrapperService dataProviderService)
        {
            this.readFileService = dataProviderService;
        }


        public async Task<byte[]?> GetImageAsync(string cmsType, Guid id, string fieldName, string? lang,
            int? width = null, int? height = null, bool cover = false, int? quality = null)
        {
            quality = quality ?? compressionQuality;
            var cmsFile = await readFileService.ReadFile(cmsType, id, fieldName, lang).ConfigureAwait(false);

            if (cmsFile == null)
                return null;

            // Read from stream.
            using (var image = Image.Load(Configuration.Default, cmsFile.Bytes, out var format))
            {
                if (cover)
                {
                    CropImage(image, width, height);
                }
                else
                {
                    ResizeImage(image, width, height);
                }

                using (var output = new MemoryStream())
                {
                    if (format.DefaultMimeType == JpegFormat.Instance.DefaultMimeType)
                    {
                        image.SaveAsJpeg(output, new JpegEncoder
                        {
                            Quality = quality,
                            Subsample = JpegSubsample.Ratio444
                        });
                    }
                    else
                    {
                        image.Save(output, format);
                    }

                    // store in cache

                    var result = output.ToArray();
                    return result;

                }
            }

            throw new Exception("You did it wrong. Find help. Or don't.");
        }

        private void CropImage(Image image, int? width, int? height)
        {
            if (!width.HasValue || !height.HasValue)
            {
                throw new ArgumentException("Both width and height are required for cover");
            }

            var newWidth = Math.Min(width.Value, image.Width);
            var newHeight = Math.Min(height.Value, image.Height);

            // convert $input -resize '$widthx$height^' -gravity center -crop '$widthx$height+0+0' $output
            var isWider = (float)image.Width / (float)image.Height > (float)newWidth / (float)newHeight;
            if (isWider)
            {
                image.Mutate(x => x.Resize(0, newHeight));
            }
            else
            {
                image.Mutate(x => x.Resize(newWidth, 0));

            }

            image.Mutate(x => x.Crop(newWidth, newHeight));

        }

        private void ResizeImage(Image image, int? width, int? height)
        {
            var newWidth = width ?? image.Width;
            var newHeight = height ?? image.Height;

            // Guard against large width from the query params
            if (newWidth < 1 || newWidth > image.Width)
                newWidth = image.Width;

            // Guard against large height from the query params
            if (newHeight < 1 || newHeight > image.Height)
                newHeight = image.Height;


            if (((double)image.Height / newHeight) > ((double)image.Width / newWidth))
            {
                newWidth = (image.Width * newHeight) / image.Height;
            }
            else
            {
                newHeight = image.Height * newWidth / image.Width;
            }

            image.Mutate(x => x.Resize(newWidth, newHeight));

            //if (width != newWidth || height != newHeight)
            //{
            //    image.Mutate(x => x.Crop(new Rectangle(
            //        width == newWidth ? 0 : (newWidth - width) / 2,
            //        height == newHeight ? 0 : (newHeight - height) / 2,
            //        width,
            //        height)));
            //}
        }
    }
}
