using Application.Common;
using Domain.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Optional;

namespace Application.Services.ImageService
{
    public class ImageService(IWebHostEnvironment webHostEnvironment) : IImageService
    {
        public async Task<Option<string>> SaveImageFromFileAsync(string path, IFormFile image, string? oldImagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    var fullOldPath = Path.Combine(webHostEnvironment.ContentRootPath, path, oldImagePath);
                    if (File.Exists(fullOldPath))
                    {
                        File.Delete(fullOldPath);
                    }
                }

                var types = image.ContentType.Split('/');

                if (types[0] != "image")
                {
                    return Option.None<string>();
                }

                var root = webHostEnvironment.ContentRootPath;
                var imageName = $"{Guid.NewGuid()}.{types[1]}";
                var filePath = Path.Combine(root, path, imageName);

                using (var stream = File.OpenWrite(filePath))
                {
                    using (var imageStream = image.OpenReadStream())
                    {
                        await imageStream.CopyToAsync(stream);
                    }
                }

                return Option.Some(imageName);
            }
            catch (Exception ex)
            {
                return Option.None<string>();
            }
        }

        public async Task<Option<List<string>>> SaveImagesFromFilesAsync(
            string path,
            IFormFileCollection images)
        {
            try
            {
                var savedImageNames = new List<string>();
                var root = webHostEnvironment.ContentRootPath;

                foreach (var image in images)
                {
                    var type = image.ContentType.Split('/');
                    if (type[0] != "image")
                    {
                        return Option.None<List<string>>();
                    }

                    var imageName = $"{Guid.NewGuid()}.{type[1]}";
                    var filePath = Path.Combine(root, path, imageName);

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    savedImageNames.Add(imageName);
                }

                return Option.Some(savedImageNames);
            }
            catch (Exception)
            {
                return Option.None<List<string>>();
            }
        }

        public async Task<Result<bool, string>> DeleteImageAsync(string path, string imagePath)
        {
            try
            {
                var fullOldPath = Path.Combine(webHostEnvironment.ContentRootPath, path, imagePath);
                if (File.Exists(fullOldPath))
                {
                    File.Delete(fullOldPath);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}