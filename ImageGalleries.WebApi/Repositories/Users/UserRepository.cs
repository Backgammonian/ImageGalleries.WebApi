using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Services.PhotoServices;
using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly IPhotoService _photoService;
        private readonly IRandomGenerator _randomGenerator;

        public UserRepository(DataContext dataContext,
            IPhotoService photoService,
            IRandomGenerator randomGenerator)
        {
            _dataContext = dataContext;
            _photoService = photoService;
            _randomGenerator = randomGenerator;
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

        public async Task<bool> UpdateProfilePicture(User user, IFormFile? formFile)
        {
            if (formFile == null)
            {
                return false;
            }

            var photoResult = await _photoService.AddPreviewPhoto(formFile);
            if (photoResult.Error != null)
            {
                return false;
            }

            user.ProfilePictureUrl = photoResult.Url.ToString();
            _dataContext.Users.Update(user);

            return await Save();
        }

        public async Task<bool> AddCommentToPicture(string userId, string pictureId, string content)
        {
            var comment = new Comment()
            {
                Id = _randomGenerator.GetRandomId(),
                PictureId = pictureId,
                UserId = userId,
                Content = content,
                CreationDate = DateTime.UtcNow
            };

            await _dataContext.Comments.AddAsync(comment);

            return await Save();
        }

        public async Task<bool> RemoveComment(string userId, string commentId)
        {
            var comment = await _dataContext.Comments.
                FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                return false;
            }

            if (comment.UserId != userId)
            {
                return false;
            }

            _dataContext.Comments.Remove(comment);

            return await Save();
        }

        public async Task<bool> AddScoreToPicture(string userId, string pictureId, int amount)
        {
            var score = new Score()
            {
                Id = _randomGenerator.GetRandomId(),
                PictureId = pictureId,
                UserId = userId,
                Amount = amount,
                CreationDate = DateTime.UtcNow
            };

            await _dataContext.Scores.AddAsync(score);

            return await Save();
        }

        public async Task<bool> RemoveScore(string userId, string scoreId)
        {
            var score = await _dataContext.Scores.
                FirstOrDefaultAsync(x => x.Id == scoreId);

            if (score == null)
            {
                return false;
            }

            if (score.UserId != userId)
            {
                return false;
            }

            _dataContext.Scores.Remove(score);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
