using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Repositories.Tags
{
    public class TagRepository : ITagRepository
    {
        private readonly DataContext _dataContext;
        private readonly IRandomGenerator _randomGenerator;

        public TagRepository(DataContext dataContext,
            IRandomGenerator randomGenerator)
        {
            _dataContext = dataContext;
            _randomGenerator = randomGenerator;
        }

        public async Task<ICollection<Tag>> GetTags()
        {
            return await _dataContext.Tags
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> DoesTagExist(string tagId)
        {
            return await _dataContext.Tags
                .AsNoTracking()
                .AnyAsync(x => x.Id == tagId);
        }

        public async Task<Tag?> GetTag(string tagId)
        {
            return await _dataContext.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == tagId);
        }

        public async Task<ICollection<Picture>> GetPicturesByTag(string tagId)
        {
            var any = await DoesTagExist(tagId);
            if (!any)
            {
                return new List<Picture>();
            }

            var picturesByTag = await _dataContext.PictureTags
                .AsNoTracking()
                .Where(x => x.TagId == tagId)
                .ToListAsync();

            var pictures = new List<Picture>();
            foreach (var pictureByTag in picturesByTag)
            {
                var picture = await _dataContext.Pictures
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == pictureByTag.PictureId);

                if (picture != null)
                {
                    pictures.Add(picture);
                }
            }

            return pictures;
        }

        public async Task<bool> CreateTag(string name, string description = "")
        {
            var tag = new Tag()
            {
                Id = _randomGenerator.GetRandomId(),
                Name = name,
                Description = description,
                CreationDate = DateTime.UtcNow
            };

            await _dataContext.Tags.AddAsync(tag);

            return await Save();
        }

        public async Task<bool> AddTagToPicture(string tagId, string pictureId)
        {
            var pictureTag = new PictureTag()
            {
                PictureId = pictureId,
                TagId = tagId
            };

            await _dataContext.PictureTags.AddAsync(pictureTag);

            return await Save();
        }

        public async Task<bool> RemoveTagFromPicture(string tagId, string pictureId)
        {
            var pictureTag = new PictureTag()
            {
                PictureId = pictureId,
                TagId = tagId
            };

            _dataContext.PictureTags.Remove(pictureTag);

            return await Save();
        }

        public async Task<bool> UpdateTagNameAndDescription(string tagId, string newName, string newDescription)
        {
            var tag = await GetTag(tagId);
            if (tag == null)
            {
                return false;
            }

            tag.Name = newName;
            tag.Description = newDescription;

            _dataContext.Tags.Update(tag);

            return await Save();
        }

        public async Task<bool> RemoveTag(string tagId)
        {
            var tag = await GetTag(tagId);
            if (tag == null)
            {
                return false;
            }

            _dataContext.Tags.Remove(tag);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
