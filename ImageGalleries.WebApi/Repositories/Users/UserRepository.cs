using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ICollection<Picture>> GetPicturesOfUser(string userId)
        {
            return await _dataContext.Pictures
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<ICollection<Gallery>> GetGalleriesOfUser(string userId)
        {
            return await _dataContext.Galleries
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<ICollection<Score>> GetScoresOfUser(string userId)
        {
            return await _dataContext.Scores
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<ICollection<Comment>> GetCommentsOfUser(string userId)
        {
            return await _dataContext.Comments
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<User?> GetUser(string userId)
        {
            return await _dataContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
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
