using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Pictures
{
    public interface IPictureRepository
    {
        Task<ICollection<Picture>> GetPictures();
        Task<ICollection<Picture>> GetPicturesOfUser(string userId);
        Task<bool> DoesPictureExist(string pictureId);
        Task<Picture?> GetPicture(string pictureId);
        Task<bool> AddPicture(IFormFile? formFile, string userId, string description = "");
        Task<bool> RemovePicture(string pictureId);
        Task<bool> UpdateProfilePicture(IFormFile? formFile, User user);
        Task<bool> Save();
    }
}