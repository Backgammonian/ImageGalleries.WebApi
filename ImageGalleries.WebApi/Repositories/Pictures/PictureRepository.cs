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

        public async Task<User?> GetUploaderOfPicture(string pictureId)
        {
            var picture = await GetPicture(pictureId);
            if (picture == null)
            {
                return null;
            }

            return await _dataContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == picture.UserId);
        }

        public async Task<ICollection<Picture>> GetPictures()
        {
            return await _dataContext.Pictures
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Picture?> GetPicture(string pictureId)
        {
            return await _dataContext.Pictures
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pictureId);
        }

        public async Task<Picture?> GetPictureTracking(string pictureId)
        {
            return await _dataContext.Pictures
                .FirstOrDefaultAsync(x => x.Id == pictureId);
        }

        public async Task<ICollection<Comment>?> GetCommentsOfPicture(string pictureId)
        {
            var picture = await GetPicture(pictureId);
            if (picture == null)
            {
                return null;
            }

            return await _dataContext.Comments
                .AsNoTracking()
                .Where(x => x.PictureId == pictureId)
                .ToListAsync();
        }

        public async Task<int> GetScoreOfPicture(string pictureId)
        {
            var any = await DoesPictureExist(pictureId);
            if (!any)
            {
                return 0;
            }

            return await _dataContext.Scores
                .AsNoTracking()
                .Where(x => x.PictureId == pictureId)
                .SumAsync(x => x.Amount);
        }

        public async Task<ICollection<Tag>?> GetTagsOfPicture(string pictureId)
        {
            var any = await DoesPictureExist(pictureId);
            if (!any)
            {
                return null;
            }

            var pictureTags = await _dataContext.PictureTags
                .AsNoTracking()
                .Where(x => x.PictureId == pictureId)
                .ToListAsync();

            var tags = new List<Tag>();
            foreach (var pictureTag in pictureTags)
            {
                var tag = await _dataContext.Tags
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == pictureTag.TagId);

                if (tag != null)
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }

        public async Task<ICollection<Gallery>?> GetGalleriesThatContainPicture(string pictureId)
        {
            var any = await DoesPictureExist(pictureId);
            if (!any)
            {
                return null;
            }

            var pictureGalleries = await _dataContext.PictureGalleries
                .AsNoTracking()
                .Where(x => x.PictureId == pictureId)
                .ToListAsync();

            var galleries = new List<Gallery>();
            foreach (var pictureGallery in pictureGalleries)
            {
                var gallery = await _dataContext.Galleries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == pictureGallery.GalleryId);

                if (gallery != null)
                {
                    galleries.Add(gallery);
                }
            }

            return galleries;
        }

        public async Task<bool> DoesPictureExist(string pictureId)
        {
            return await _dataContext.Pictures
                .AsNoTracking()
                .AnyAsync(x => x.Id == pictureId);
        }

        public async Task<bool> AddPicture(IFormFile formFile, string userId, string description = "")
        {
            var photoResult = await _photoService.AddPhoto(formFile);
            if (photoResult.Error != null)
            {
                return false;
            }

            var previewPhotoResult = await _photoService.AddPreviewPhoto(formFile);
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

        public async Task<bool> RemovePicture(Picture picture)
        {
            var pictureDeletionResult = await _photoService.DeletePhoto(picture.Url);
            if (pictureDeletionResult.Error != null)
            {
                return false;
            }

            var previewDeletionResult = await _photoService.DeletePhoto(picture.PreviewUrl);
            if (previewDeletionResult.Error != null)
            {
                return false;
            }

            _dataContext.Pictures.Remove(picture);

            return await Save();
        }

        public async Task<bool> UpdatePictureDescription(Picture picture, string description)
        {
            picture.Description = description;
            _dataContext.Pictures.Update(picture);
            
            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}