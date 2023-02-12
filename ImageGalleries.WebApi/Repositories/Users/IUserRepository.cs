using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Users
{
    public interface IUserRepository
    {
        Task<bool> UpdateUsername(User user, string newUsername);
        Task<bool> Save();
    }
}
