﻿using Application.Common;
using Microsoft.AspNetCore.Http;
using Optional;

namespace Application.Services.ImageService
{
    public interface IImageService
    {
        Task<string?> SaveImageFromFileAsync(string path, IFormFile image, string? oldImagePath = null);
        Task<List<string?>> SaveImagesFromFilesAsync(string path, IFormFileCollection images);
        Result<bool, string> DeleteImageAsync(string path, string imageName);
    }
}