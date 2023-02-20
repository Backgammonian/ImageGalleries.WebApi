using CloudinaryDotNet.Actions;
using FakeItEasy;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Galleries;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Services.PhotoServices;
using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly TestDatabaseGenerator _dbGenerator;
        private readonly IPhotoService _photoService;
        private readonly IRandomGenerator _randomGenerator;
        private UserRepository? _repo;

        public UserRepositoryTests()
        {
            _dbGenerator = new TestDatabaseGenerator();
            _photoService = A.Fake<IPhotoService>();
            _randomGenerator = A.Fake<IRandomGenerator>();
        }

        [Fact]
        public async Task UserRepository_UpdateUsername_ReturnsSuccess()
        {
            var dbContext = await _dbGenerator.GetDatabase();
            _repo = new UserRepository(dbContext, _photoService, _randomGenerator);
            var newUsername = "NewUsername";
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == "user1");
            dbContext.ChangeTracker.Clear();

            var result = await _repo.UpdateUsername(user, newUsername);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_UpdateProfilePicture_ReturnsSuccess()
        {
            var dbContext = await _dbGenerator.GetDatabase();
            _repo = new UserRepository(dbContext, _photoService, _randomGenerator);
            var formFile = A.Fake<IFormFile>();
            var photoResult = A.Fake<ImageUploadResult>();
            photoResult.Url = new Uri("picture.jpg", UriKind.Relative);
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == "user1");
            dbContext.ChangeTracker.Clear();
            A.CallTo(() => _photoService.AddPreviewPhoto(formFile)).Returns(photoResult);

            var result = await _repo.UpdateProfilePicture(user, formFile);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_AddCommentToPicture_ReturnsSuccess()
        {
            _repo = new UserRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var userId = "user1";
            var pictureId = "1";
            var content = "Comment";

            var result = await _repo.AddCommentToPicture(userId, pictureId, content);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_RemoveComment_ReturnsSuccess()
        {
            var dbContext = await _dbGenerator.GetDatabase();
            _repo = new UserRepository(dbContext, _photoService, _randomGenerator);
            var comment = await _repo.GetComment("0");
            dbContext.ChangeTracker.Clear();

            var result = await _repo.RemoveComment(comment);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_AddScoreToPicture_ReturnsSuccess()
        {
            var dbContext = await _dbGenerator.GetDatabase();
            _repo = new UserRepository(dbContext, _photoService, _randomGenerator);
            var userId = "user1";
            var pictureId = "3";
            var amount = 1;

            var result = await _repo.AddScoreToPicture(userId, pictureId, amount);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_RemoveScoreFromPicture_ReturnsSuccess()
        {
            var dbContext = await _dbGenerator.GetDatabase();
            _repo = new UserRepository(dbContext, _photoService, _randomGenerator);
            var userId = "user1";
            var pictureId = "1";
            var score = await _repo.GetScore(userId, pictureId);
            dbContext.ChangeTracker.Clear();

            var result = await _repo.RemoveScoreFromPicture(score);

            result.Should().BeTrue();
        }
    }
}
