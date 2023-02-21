using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Galleries
{
    public interface IGalleryRepository
    {
        Task<ICollection<Gallery>> GetGalleries();
        Task<bool> DoesGalleryExist(string galleryId);
        Task<Gallery?> GetGallery(string galleryId);
        Task<Gallery?> GetGalleryTracking(string galleryId);
        Task<User?> GetGalleryOwner(string galleryId);
        Task<ICollection<Picture>?> GetPicturesFromGallery(string galleryId);
        Task<bool> CreateGallery(string userId, string galleryName, string galleryDescription = "");
        Task<bool> AddPictureToGallery(Gallery gallery, string pictureId);
        Task<bool> RemovePictureFromGallery(Gallery gallery, string pictureId);
        Task<bool> UpdateGalleryNameAndDescription(Gallery gallery, string newName, string newDescription);
        Task<bool> RemoveGallery(Gallery gallery);
        Task<bool> Save();
    }
}
