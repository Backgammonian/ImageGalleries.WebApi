using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Repositories.Galleries
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly DataContext _dataContext;
        private readonly IRandomGenerator _randomGenerator;

        public GalleryRepository(DataContext dataContext,
            IRandomGenerator randomGenerator)
        {
            _dataContext = dataContext;
            _randomGenerator = randomGenerator;
        }

        public async Task<ICollection<Gallery>> GetGalleries()
        {
            return await _dataContext.Galleries
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> DoesGalleryExist(string galleryId)
        {
            return await _dataContext.Galleries
                .AsNoTracking()
                .AnyAsync(x => x.Id == galleryId);
        }

        public async Task<Gallery?> GetGallery(string galleryId)
        {
            return await _dataContext.Galleries
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == galleryId);
        }

        public async Task<User?> GetGalleryOwner(string galleryId)
        {
            var gallery = await GetGallery(galleryId);
            if (gallery == null)
            {
                return null;
            }

            return await _dataContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == gallery.UserId);
        }

        public async Task<ICollection<Picture>> GetPicturesFromGallery(string galleryId)
        {
            var any = await DoesGalleryExist(galleryId);
            if (!any)
            {
                return new List<Picture>();
            }

            var picturesFromGallery = await _dataContext.PictureGalleries
                .AsNoTracking()
                .Where(x => x.GalleryId == galleryId)
                .ToListAsync();

            var pictures = new List<Picture>();
            foreach (var pictureFromGallery in picturesFromGallery)
            {
                var picture = await _dataContext.Pictures
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == pictureFromGallery.PictureId);

                if (picture != null)
                {
                    pictures.Add(picture);
                }
            }

            return pictures;
        }

        public async Task<bool> CreateGallery(string userId, string galleryName, string galleryDescription = "")
        {
            var gallery = new Gallery()
            {
                Id = _randomGenerator.GetRandomId(),
                Name = galleryName,
                Description = galleryDescription,
                CreationDate = DateTime.UtcNow,
                UserId = userId
            };

            await _dataContext.Galleries.AddAsync(gallery);

            return await Save();
        }

        public async Task<bool> AddPictureToGallery(string userId, string galleryId, string pictureId)
        {
            var gallery = await GetGallery(galleryId);
            if (gallery == null)
            {
                return false;
            }

            if (gallery.UserId != userId)
            {
                return false;
            }

            var pictureGallery = new PictureGallery()
            {
                PictureId = pictureId,
                GalleryId = galleryId
            };

            await _dataContext.PictureGalleries.AddAsync(pictureGallery);

            return await Save();
        }

        public async Task<bool> RemovePictureFromGallery(string userId, string galleryId, string pictureId)
        {
            var gallery = await GetGallery(galleryId);
            if (gallery == null)
            {
                return false;
            }

            if (gallery.UserId != userId)
            {
                return false;
            }

            var pictureGallery = new PictureGallery()
            {
                PictureId = pictureId,
                GalleryId = galleryId
            };

            _dataContext.PictureGalleries.Remove(pictureGallery);

            return await Save();
        }

        public async Task<bool> UpdateGalleryNameAndDescription(string userId, string galleryId, string newName, string newDescription)
        {
            var gallery = await GetGallery(galleryId);
            if (gallery == null)
            {
                return false;
            }

            if (gallery.UserId != userId)
            {
                return false;
            }

            gallery.Name = newName;
            gallery.Description = newDescription;
            
            _dataContext.Galleries.Update(gallery);

            return await Save();
        }

        public async Task<bool> RemoveGallery(string userId, string galleryId)
        {
            var gallery = await GetGallery(galleryId);
            if (gallery == null)
            {
                return false;
            }

            if (gallery.UserId != userId)
            {
                return false;
            }

            _dataContext.Galleries.Remove(gallery);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
