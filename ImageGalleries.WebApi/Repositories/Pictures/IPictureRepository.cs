using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Pictures
{
    public interface IPictureRepository
    {
        Task<User?> GetUploaderOfPicture(string pictureId);
        Task<ICollection<Picture>> GetPictures();
        Task<ICollection<Comment>> GetCommentsOfPicture(string pictureId);
        Task<int> GetScoreOfPicture(string pictureId);
        Task<ICollection<Tag>> GetTagsOfPicture(string pictureId);
        Task<ICollection<Gallery>> GetGalleriesThatContainPicture(string pictureId);
        Task<bool> DoesPictureExist(string pictureId);
        Task<Picture?> GetPicture(string pictureId);
        Task<bool> AddPicture(IFormFile formFile, string userId, string description = "");
        Task<bool> RemovePicture(Picture picture);
        Task<bool> UpdatePictureDescription(Picture picture, string description);
        Task<bool> Save();
    }
}