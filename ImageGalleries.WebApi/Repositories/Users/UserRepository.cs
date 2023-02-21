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

        public async Task<ICollection<Picture>?> GetPicturesOfUser(string userId)
        {
            var any = await DoesUserExist(userId);
            if (!any)
            {
                return null;
            }

            return await _dataContext.Pictures
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<ICollection<Gallery>?> GetGalleriesOfUser(string userId)
        {
            var any = await DoesUserExist(userId);
            if (!any)
            {
                return null;
            }

            return await _dataContext.Galleries
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<ICollection<Score>?> GetScoresOfUser(string userId)
        {
            var any = await DoesUserExist(userId);
            if (!any)
            {
                return null;
            }

            return await _dataContext.Scores
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<ICollection<Comment>?> GetCommentsOfUser(string userId)
        {
            var any = await DoesUserExist(userId);
            if (!any)
            {
                return null;
            }

            return await _dataContext.Comments
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> DoesUserExist(string userId)
        {
            return await _dataContext.Users
                .AsNoTracking()
                .AnyAsync(x => x.Id == userId);
        }

        public async Task<User?> GetUser(string userId)
        {
            return await _dataContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<User?> GetUserTracking(string userId)
        {
            return await _dataContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<bool> UpdateUsername(User user, string newUsername)
        {
            user.UserName = newUsername;
            user.NormalizedUserName = newUsername.ToUpperInvariant();
            _dataContext.Users.Update(user);

            return await Save();
        }

        public async Task<bool> UpdateProfilePicture(User user, IFormFile formFile)
        {
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
            var any = await _dataContext.Pictures
                .AsNoTracking()
                .AnyAsync(x => x.Id == pictureId);

            if (!any)
            {
                return false;
            }

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

        public async Task<bool> RemoveComment(Comment comment)
        {
            _dataContext.Comments.Remove(comment);

            return await Save();
        }

        public async Task<Comment?> GetCommentTracking(string commentId)
        {
            return await _dataContext.Comments
                .FirstOrDefaultAsync(x => x.Id == commentId);
        }

        public async Task<bool> AddScoreToPicture(string userId, string pictureId, int amount)
        {
            var any = await _dataContext.Pictures
                .AsNoTracking()
                .AnyAsync(x => x.Id == pictureId);

            if (!any)
            {
                return false;
            }

            var score = new Score()
            {
                PictureId = pictureId,
                UserId = userId,
                Amount = amount,
                CreationDate = DateTime.UtcNow
            };

            await _dataContext.Scores.AddAsync(score);

            return await Save();
        }

        public async Task<bool> RemoveScoreFromPicture(Score score)
        {
            _dataContext.Scores.Remove(score);

            return await Save();
        }

        public async Task<bool> DoesScoreExist(string userId, string pictureId)
        {
            return await _dataContext.Scores
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId &&
                    x.PictureId == pictureId);
        }

        public async Task<Score?> GetScoreTracking(string userId, string pictureId)
        {
            var doesScoreExist = await DoesScoreExist(userId, pictureId);
            if (doesScoreExist)
            {
                return await _dataContext.Scores
                    .FirstOrDefaultAsync(x => x.UserId == userId &&
                        x.PictureId == pictureId);
            }

            return null;
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
