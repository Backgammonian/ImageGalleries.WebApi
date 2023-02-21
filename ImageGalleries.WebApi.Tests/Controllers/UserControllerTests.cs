using AutoMapper;
using ImageGalleries.WebApi.Controllers;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly TestPictureGenerator _testPictureGenerator;
        private UserController? _userController;

        public UserControllerTests()
        {
            _userRepository = A.Fake<IUserRepository>();
            _mapper = A.Fake<IMapper>();
            _testPictureGenerator = new TestPictureGenerator();
        }

        private UserController GetControllerWithContext(string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim("UserId", userId)
            };
            var httpContext = new DefaultHttpContext();
            httpContext.User.AddIdentity(new ClaimsIdentity(claims));
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            return new UserController(_userRepository,
                _mapper)
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async Task UserController_ChangeProfilePicture_ReturnsOK()
        {
            var userId = "1";
            _userController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var changeProfilePictureRequest = new ChangeProfilePictureRequest()
            {
                ProfilePicture = _testPictureGenerator.GetPicture()
            };
            var profilePicture = changeProfilePictureRequest.ProfilePicture;
            A.CallTo(() => _userRepository.GetUserTracking(userId)).Returns(user);
            A.CallTo(() => _userRepository.UpdateProfilePicture(user, profilePicture)).Returns(true);

            var result = await _userController.ChangeProfilePicture(changeProfilePictureRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UserController_ChangeUsername_ReturnsOK()
        {
            var userId = "1";
            _userController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var changeUsernameRequest = new ChangeUsernameRequest()
            {
                NewUsername = "new-username"
            };
            var newUsername = changeUsernameRequest.NewUsername;
            A.CallTo(() => _userRepository.GetUserTracking(userId)).Returns(user);
            A.CallTo(() => _userRepository.UpdateUsername(user, newUsername)).Returns(true);

            var result = await _userController.ChangeUsername(changeUsernameRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UserController_AddCommentToPicture_ReturnsOK()
        {
            var userId = "1";
            _userController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var addCommentRequest = new AddCommentRequest()
            {
                PictureId = "1",
                Content = "content"
            };
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _userRepository.AddCommentToPicture(userId,
                addCommentRequest.PictureId,
                addCommentRequest.Content)).Returns(true);

            var result = await _userController.AddCommentToPicture(addCommentRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UserController_RemoveComment_ReturnsOK()
        {
            var userId = "1";
            _userController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var comment = A.Fake<Comment>();
            comment.UserId = userId;
            var removeCommentRequest = new RemoveCommentRequest()
            {
                CommentId = "1"
            };
            var commentId = removeCommentRequest.CommentId;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _userRepository.GetCommentTracking(commentId)).Returns(comment);
            A.CallTo(() => _userRepository.RemoveComment(comment)).Returns(true);

            var result = await _userController.RemoveComment(removeCommentRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UserController_AddScoreToPicture_ReturnsOK()
        {
            var userId = "1";
            _userController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var score = A.Fake<Score>();
            var addScoreRequest = new AddScoreRequest()
            {
                PictureId = "1",
                IsUpvote = true
            };
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _userRepository.GetScoreTracking(userId, addScoreRequest.PictureId)).Returns(score);
            A.CallTo(() => _userRepository.AddScoreToPicture(userId,
                addScoreRequest.PictureId,
                addScoreRequest.IsUpvote ? 1 : -1)).Returns(true);

            var result = await _userController.AddScoreToPicture(addScoreRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
