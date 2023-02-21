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

        public async Task<bool> DoesTagExist(string tagName)
        {
            return await _dataContext.Tags
                .AsNoTracking()
                .AnyAsync(x => x.Name == tagName);
        }

        public async Task<Tag?> GetTag(string tagName)
        {
            return await _dataContext.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == tagName);
        }

        public async Task<Tag?> GetTagTracking(string tagName)
        {
            return await _dataContext.Tags
                .FirstOrDefaultAsync(x => x.Name == tagName);
        }

        public async Task<ICollection<Picture>?> GetPicturesByTag(string tagName)
        {
            var tag = await GetTag(tagName);
            if (tag == null)
            {
                return null;
            }

            var picturesByTag = await _dataContext.PictureTags
                .AsNoTracking()
                .Where(x => x.TagId == tag.Id)
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
            var any = await DoesTagExist(name);
            if (any)
            {
                return false;
            }

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

        public async Task<bool> AddTagToPicture(Tag tag, Picture picture)
        {
            var any = await _dataContext.PictureTags
                .AsNoTracking()
                .AnyAsync(x => x.TagId == tag.Id &&
                    x.PictureId == picture.Id);

            if (any)
            {
                return false;
            }

            var pictureTag = new PictureTag()
            {
                PictureId = picture.Id,
                TagId = tag.Id
            };

            await _dataContext.PictureTags.AddAsync(pictureTag);

            return await Save();
        }

        public async Task<bool> RemoveTagFromPicture(Tag tag, Picture picture)
        {
            var pictureTag = await _dataContext.PictureTags
                .FirstOrDefaultAsync(x => x.TagId == tag.Id &&
                    x.PictureId == picture.Id);

            if (pictureTag == null)
            {
                return false;
            }

            _dataContext.PictureTags.Remove(pictureTag);

            return await Save();
        }

        public async Task<bool> UpdateTag(Tag tag, string newName, string newDescription)
        {
            if (await DoesTagExist(newName))
            {
                return false;
            }

            tag.Name = newName;
            tag.Description = newDescription;

            _dataContext.Tags.Update(tag);

            return await Save();
        }

        public async Task<bool> RemoveTag(Tag tag)
        {
            _dataContext.Tags.Remove(tag);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
