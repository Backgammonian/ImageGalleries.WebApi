using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> UpdateUsername(User user, string newUsername)
        {
            user.UserName = newUsername;
            user.NormalizedUserName = newUsername.ToUpperInvariant();
            _dataContext.Users.Update(user);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
