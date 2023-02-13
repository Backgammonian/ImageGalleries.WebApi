using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Galleries
{
    public interface IGalleryRepository
    {
        Task<ICollection<Gallery>> GetGalleries();
        Task<bool> DoesGalleryExist(string galleryId);
        Task<Gallery?> GetGallery(string galleryId);
        Task<User?> GetGalleryOwner(string galleryId);
        Task<ICollection<Picture>> GetPicturesFromGallery(string galleryId);
        Task<bool> CreateGallery(string userId, string galleryName, string galleryDescription = "");
        Task<bool> AddPictureToGallery(string galleryId, string pictureId);
        Task<bool> RemovePictureFromGallery(string galleryId, string pictureId);
        Task<bool> UpdateGalleryNameAndDescription(string galleryId, string newName, string newDescription);
        Task<bool> RemoveGallery(string galleryId);
        Task<bool> Save();
    }
}
