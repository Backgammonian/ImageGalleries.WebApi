using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Users
{
    public interface IUserRepository
    {
        Task<ICollection<Picture>> GetPicturesOfUser(string userId);
        Task<ICollection<Gallery>> GetGalleriesOfUser(string userId);
        Task<ICollection<Score>> GetScoresOfUser(string userId);
        Task<ICollection<Comment>> GetCommentsOfUser(string userId);
        Task<User?> GetUser(string userId);
        Task<bool> UpdateUsername(User user, string newUsername);
        Task<bool> UpdateProfilePicture(User user, IFormFile? formFile);
        Task<bool> AddCommentToPicture(string userId, string pictureId, string content);
        Task<bool> RemoveComment(string userId, string commentId);
        Task<bool> AddScoreToPicture(string userId, string pictureId, int amount);
        Task<bool> RemoveScore(string userId, string scoreId);
        Task<bool> Save();
    }
}
