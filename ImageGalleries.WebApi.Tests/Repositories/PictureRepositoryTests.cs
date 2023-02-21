using CloudinaryDotNet.Actions;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Pictures;
using ImageGalleries.WebApi.Services.PhotoServices;
using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.AspNetCore.Http;

namespace ImageGalleries.WebApi.Tests.Repositories
{
    public class PictureRepositoryTests
    {
        private readonly TestDatabaseGenerator _dbGenerator;
        private readonly IPhotoService _photoService;
        private readonly IRandomGenerator _randomGenerator;
        private PictureRepository? _repo;

        public PictureRepositoryTests()
        {
            _dbGenerator = new TestDatabaseGenerator();
            _photoService = A.Fake<IPhotoService>();
            _randomGenerator = A.Fake<IRandomGenerator>();
        }

        [Fact]
        public async Task PictureRepository_GetTagsOfPicture_ReturnsSuccess()
        {
            _repo = new PictureRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var pictureId = "1";

            var result = await _repo.GetTagsOfPicture(pictureId);

            result.Should().NotBeNull();
            result.Should().HaveCountGreaterThan(0);
            result.Should().BeAssignableTo<ICollection<Tag>>();
        }

        [Fact]
        public async Task PictureRepository_GetGalleriesThatContainPicture_ReturnsSuccess()
        {
            _repo = new PictureRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var pictureId = "1";

            var result = await _repo.GetGalleriesThatContainPicture(pictureId);

            result.Should().NotBeNull();
            result.Should().HaveCountGreaterThan(0);
            result.Should().BeAssignableTo<ICollection<Gallery>>();
        }

        [Fact]
        public async Task PictureRepository_AddPicture_ReturnsSuccess()
        {
            _repo = new PictureRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var userId = "user1";
            var formFile = A.Fake<IFormFile>();
            var description = "description";
            var photoResult = A.Fake<ImageUploadResult>();
            photoResult.Url = new Uri("picture.jpg", UriKind.Relative);
            var previewPhotoResult = A.Fake<ImageUploadResult>();
            previewPhotoResult.Url = new Uri("preview-picture.jpg", UriKind.Relative);
            A.CallTo(() => _photoService.AddPhoto(formFile)).Returns(photoResult);
            A.CallTo(() => _photoService.AddPreviewPhoto(formFile)).Returns(previewPhotoResult);
            var oldCount = (await _repo.GetPictures()).Count;

            var result = await _repo.AddPicture(formFile, userId, description); 
            var newCount = (await _repo.GetPictures()).Count;

            result.Should().BeTrue();
            newCount.Should().Be(oldCount + 1);
        }

        [Fact]
        public async Task PictureRepository_RemovePicture_ReturnsSuccess()
        {
            _repo = new PictureRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var picture = await _repo.GetPictureTracking("1");
            var photoResult = A.Fake<DeletionResult>();
            var previewPhotoResult = A.Fake<DeletionResult>();
            A.CallTo(() => _photoService.DeletePhoto(picture.Url)).Returns(photoResult);
            A.CallTo(() => _photoService.DeletePhoto(picture.PreviewUrl)).Returns(previewPhotoResult);
            var oldCount = (await _repo.GetPictures()).Count;

            var result = await _repo.RemovePicture(picture);
            var newCount = (await _repo.GetPictures()).Count;

            result.Should().BeTrue();
            newCount.Should().Be(oldCount - 1);
        }

        [Fact]
        public async Task PictureRepository_UpdatePictureDescription_ReturnsSuccess()
        {
            _repo = new PictureRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var newDescription = "new-description";
            var pictureId = "1";
            var picture = await _repo.GetPictureTracking(pictureId);

            var result = await _repo.UpdatePictureDescription(picture, newDescription);
            var changedEntity = await _repo.GetPicture(pictureId);

            result.Should().BeTrue();
            changedEntity.Description.Should().Be(newDescription);
        }
    }
}
