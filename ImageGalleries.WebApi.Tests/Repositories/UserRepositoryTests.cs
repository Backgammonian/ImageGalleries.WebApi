using CloudinaryDotNet.Actions;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Services.PhotoServices;
using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.AspNetCore.Http;

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
            _repo = new UserRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var newUsername = "NewUsername";
            var userId = "user1";
            var user = await _repo.GetUserTracking(userId);

            var result = await _repo.UpdateUsername(user, newUsername);
            var changedEntity = await _repo.GetUser(userId);

            result.Should().BeTrue();
            changedEntity.UserName.Should().Be(newUsername);
        }

        [Fact]
        public async Task UserRepository_UpdateProfilePicture_ReturnsSuccess()
        {
            _repo = new UserRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var formFile = A.Fake<IFormFile>();
            var photoResult = A.Fake<ImageUploadResult>();
            photoResult.Url = new Uri("picture.jpg", UriKind.Relative);
            var userId = "user1";
            var user = await _repo.GetUserTracking(userId);
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
            _repo = new UserRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var commentId = "0";
            var comment = await _repo.GetCommentTracking(commentId);

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
            _repo = new UserRepository(await _dbGenerator.GetDatabase(),
                _photoService,
                _randomGenerator);
            var userId = "user1";
            var pictureId = "1";
            var score = await _repo.GetScoreTracking(userId, pictureId);

            var result = await _repo.RemoveScoreFromPicture(score);

            result.Should().BeTrue();
        }
    }
}
