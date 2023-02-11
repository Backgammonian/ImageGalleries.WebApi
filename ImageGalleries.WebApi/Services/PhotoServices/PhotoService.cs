using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using ImageGalleries.WebApi.Models;
using Microsoft.Extensions.Options;

namespace ImageGalleries.WebApi.Services.PhotoServices
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloundinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);

            _cloundinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation()
                };

                uploadResult = await _cloundinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string url)
        {
            var publicId = url.Split('/').Last().Split('.')[0];
            var deleteParams = new DeletionParams(publicId);

            return await _cloundinary.DestroyAsync(deleteParams);
        }
    }
}
