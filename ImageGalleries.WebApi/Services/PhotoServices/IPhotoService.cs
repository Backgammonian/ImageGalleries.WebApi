﻿using CloudinaryDotNet.Actions;

namespace ImageGalleries.WebApi.Services.PhotoServices
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string url);
    }
}