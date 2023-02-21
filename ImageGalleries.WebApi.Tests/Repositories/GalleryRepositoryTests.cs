using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Galleries;
using ImageGalleries.WebApi.Services.RandomGenerators;

namespace ImageGalleries.WebApi.Tests.Repositories
{
    public class GalleryRepositoryTests
    {
        private readonly TestDatabaseGenerator _dbGenerator;
        private readonly IRandomGenerator _randomGenerator;
        private GalleryRepository? _repo;

        public GalleryRepositoryTests()
        {
            _dbGenerator = new TestDatabaseGenerator();
            _randomGenerator = A.Fake<IRandomGenerator>();
        }

        [Fact]
        public async Task GalleryRepository_GetPicturesFromGallery_ReturnsSuccess()
        {
            _repo = new GalleryRepository(await _dbGenerator.GetDatabase(),
                _randomGenerator);
            var galleryId = "1";

            var result = await _repo.GetPicturesFromGallery(galleryId);

            result.Should().NotBeNull();
            result.Should().HaveCountGreaterThan(0);
            result.Should().BeAssignableTo<ICollection<Picture>>();
        }

        [Fact]
        public async Task GalleryRepository_CreateGallery_ReturnsSuccess()
        {
            _repo = new GalleryRepository(await _dbGenerator.GetDatabase(),
                _randomGenerator);
            var userId = "user1";
            var galleryName = "name";
            var galleryDescription = "description";
            var oldCount = (await _repo.GetGalleries()).Count;

            var result = await _repo.CreateGallery(userId, galleryName, galleryDescription);
            var newCount = (await _repo.GetGalleries()).Count;

            result.Should().BeTrue();
            newCount.Should().Be(oldCount + 1);
        } 
        
        [Fact]
        public async Task GalleryRepository_AddPictureToGallery_ReturnsSuccess()
        {
            _repo = new GalleryRepository(await _dbGenerator.GetDatabase(),
                _randomGenerator);
            var pictureId = "4";
            var gallery = await _repo.GetGallery("1");

            var result = await _repo.AddPictureToGallery(gallery, pictureId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GalleryRepository_RemovePictureFromGallery_ReturnsSuccess()
        {
            _repo = new GalleryRepository(await _dbGenerator.GetDatabase(),
                _randomGenerator);
            var pictureId = "1";
            var gallery = await _repo.GetGallery("1");

            var result = await _repo.RemovePictureFromGallery(gallery, pictureId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GalleryRepository_UpdateGalleryNameAndDescription_ReturnsSuccess()
        {
            _repo = new GalleryRepository(await _dbGenerator.GetDatabase(),
                _randomGenerator);
            var newName = "new-name";
            var newDescription = "new-description";
            var galleryId = "1";
            var gallery = await _repo.GetGalleryTracking(galleryId);

            var result = await _repo.UpdateGalleryNameAndDescription(gallery, newName, newDescription);
            var changedEntity = await _repo.GetGallery(galleryId);

            result.Should().BeTrue();
            changedEntity.Name.Should().Be(newName);
            changedEntity.Description.Should().Be(newDescription);
        }

        [Fact]
        public async Task GalleryRepository_RemoveGallery_ReturnsSuccess()
        {
            _repo = new GalleryRepository(await _dbGenerator.GetDatabase(),
                _randomGenerator);
            var gallery = await _repo.GetGalleryTracking("1");
            var oldCount = (await _repo.GetGalleries()).Count;

            var result = await _repo.RemoveGallery(gallery);
            var newCount = (await _repo.GetGalleries()).Count;

            result.Should().BeTrue();
            newCount.Should().Be(oldCount - 1);
        }
    }
}
