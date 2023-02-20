using CloudinaryDotNet.Actions;
using FakeItEasy;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Galleries;
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

            var result = await _repo.AddPicture(formFile, userId, description);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PictureRepository_RemovePicture_ReturnsSuccess()
        {
            var dbContext = await _dbGenerator.GetDatabase();
            _repo = new PictureRepository(dbContext, _photoService, _randomGenerator);
            var picture = await _repo.GetPicture("1");
            dbContext.ChangeTracker.Clear();
            var photoResult = A.Fake<DeletionResult>();
            var previewPhotoResult = A.Fake<DeletionResult>();
            A.CallTo(() => _photoService.DeletePhoto(picture.Url)).Returns(photoResult);
            A.CallTo(() => _photoService.DeletePhoto(picture.PreviewUrl)).Returns(previewPhotoResult);

            var result = await _repo.RemovePicture(picture);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PictureRepository_UpdatePictureDescription_ReturnsSuccess()
        {
            var dbContext = await _dbGenerator.GetDatabase();
            _repo = new PictureRepository(dbContext, _photoService, _randomGenerator);
            var newDescription = "new-description";
            var picture = await _repo.GetPicture("1");
            dbContext.ChangeTracker.Clear();

            var result = await _repo.UpdatePictureDescription(picture, newDescription);

            result.Should().BeTrue();
        }
    }
}
