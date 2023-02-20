using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Galleries;
using ImageGalleries.WebApi.Repositories.Tags;
using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Tests.Repositories
{
    public class TagRepositoryTests
    {
        private readonly TestDatabaseGenerator _dbGenerator;
        private readonly IRandomGenerator _randomGenerator;
        private TagRepository? _repo;

        public TagRepositoryTests()
        {
            _dbGenerator = new TestDatabaseGenerator();
            _randomGenerator = A.Fake<IRandomGenerator>();
        }

        [Fact]
        public async Task TagRepository_GetPicturesByTag_ReturnsSuccess()
        {
            _repo = new TagRepository(await _dbGenerator.GetDatabase(),
                _randomGenerator);
            var tagName = "Tag 1";

            var result = await _repo.GetPicturesByTag(tagName);

            result.Should().NotBeNull();
            result.Should().HaveCountGreaterThan(0);
            result.Should().BeAssignableTo<ICollection<Picture>>();
        }

        [Fact]
        public async Task TagRepository_CreateTag_ReturnsSuccess()
        {
            _repo = new TagRepository(await _dbGenerator.GetDatabase(),
                _randomGenerator);
            var tagName = "New Tag";
            var tagDescription = "Description";

            var result = await _repo.CreateTag(tagName, tagDescription);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task TagRepository_AddTagToPicture_ReturnsSuccess()
        {
            var dataContext = await _dbGenerator.GetDatabase();
            _repo = new TagRepository(dataContext, _randomGenerator);
            var picture = await dataContext.Pictures.AsNoTracking().FirstOrDefaultAsync(x => x.Id == "4");
            var tag = await _repo.GetTag("Tag 1");

            var result = await _repo.AddTagToPicture(tag, picture);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task TagRepository_RemoveTagToPicture_ReturnsSuccess()
        {
            var dataContext = await _dbGenerator.GetDatabase();
            _repo = new TagRepository(dataContext, _randomGenerator);
            var picture = await dataContext.Pictures.AsNoTracking().FirstOrDefaultAsync(x => x.Id == "1");
            var tag = await _repo.GetTag("Tag 1");
            dataContext.ChangeTracker.Clear();

            var result = await _repo.RemoveTagFromPicture(tag, picture);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task TagRepository_UpdateTag_ReturnsSuccess()
        {
            var dbContext = await _dbGenerator.GetDatabase();
            _repo = new TagRepository(dbContext, _randomGenerator);
            var newName = "new-name";
            var newDescription = "new-description";
            var tag = await _repo.GetTag("Tag 1");
            dbContext.ChangeTracker.Clear();

            var result = await _repo.UpdateTag(tag, newName, newDescription);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task TagRepository_RemoveTag_ReturnsSuccess()
        {
            var dbContext = await _dbGenerator.GetDatabase();
            _repo = new TagRepository(dbContext, _randomGenerator);
            var tag = await _repo.GetTag("Tag 1");
            dbContext.ChangeTracker.Clear();

            var result = await _repo.RemoveTag(tag);

            result.Should().BeTrue();
        }
    }
}
