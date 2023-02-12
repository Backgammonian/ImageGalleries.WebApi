using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Services.PhotoServices;
using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Repositories.Pictures
{
    public class PictureRepository : IPictureRepository
    {
        private readonly DataContext _dataContext;
        private readonly IPhotoService _photoService;
        private readonly IRandomGenerator _randomGenerator;

        public PictureRepository(DataContext dataContext,
            IPhotoService photoService,
            IRandomGenerator randomGenerator)
        {
            _dataContext = dataContext;
            _photoService = photoService;
            _randomGenerator = randomGenerator;
        }

        public async Task<ICollection<Picture>> GetPictures()
        {
            return await _dataContext.Pictures
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<Picture>> GetPicturesOfUser(string userId)
        {
            return await _dataContext.Pictures
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Picture?> GetPicture(string pictureId)
        {
            return await _dataContext.Pictures
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pictureId);
        }

        public async Task<bool> DoesPictureExist(string pictureId)
        {
            return await _dataContext.Pictures
                .AsNoTracking()
                .AnyAsync(x => x.Id == pictureId);
        }

        public async Task<bool> AddPicture(IFormFile? formFile, string userId, string description = "")
        {
            if (formFile == null)
            {
                return false;
            }

            var photoResult = await _photoService.AddPhotoAsync(formFile);
            if (photoResult.Error != null)
            {
                return false;
            }

            var previewPhotoResult = await _photoService.AddPreviewPhotoAsync(formFile);
            if (previewPhotoResult.Error != null)
            {
                return false;
            }

            var picture = new Picture()
            {
                Id = _randomGenerator.GetRandomId(),
                Url = photoResult.Url.ToString(),
                PreviewUrl = previewPhotoResult.Url.ToString(),
                UploadTime = DateTime.UtcNow,
                Description = description,
                UserId = userId
            };
            await _dataContext.Pictures.AddAsync(picture);

            return await Save();
        }

        public async Task<bool> RemovePicture(string pictureId)
        {
            var picture = await GetPicture(pictureId);
            if (picture == null)
            {
                return false;
            }

            var pictureDeletionResult = await _photoService.DeletePhotoAsync(picture.Url);
            if (pictureDeletionResult.Error != null)
            {
                return false;
            }

            var previewDeletionResult = await _photoService.DeletePhotoAsync(picture.PreviewUrl);
            if (previewDeletionResult.Error != null)
            {
                return false;
            }

            _dataContext.Pictures.Remove(picture);

            return await Save();
        }

        public async Task<bool> UpdateProfilePicture(IFormFile? formFile, User user)
        {
            if (formFile == null)
            {
                return false;
            }

            var photoResult = await _photoService.AddPreviewPhotoAsync(formFile);
            if (photoResult.Error != null)
            {
                return false;
            }

            user.ProfilePictureUrl = photoResult.Url.ToString();
            _dataContext.Users.Update(user);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}