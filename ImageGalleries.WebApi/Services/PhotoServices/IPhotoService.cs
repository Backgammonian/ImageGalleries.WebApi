using CloudinaryDotNet.Actions;

namespace ImageGalleries.WebApi.Services.PhotoServices
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhoto(IFormFile file);
        Task<ImageUploadResult> AddPreviewPhoto(IFormFile file);
        Task<DeletionResult> DeletePhoto(string url);
    }
}
