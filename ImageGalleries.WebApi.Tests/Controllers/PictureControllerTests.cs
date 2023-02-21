using AutoMapper;
using ImageGalleries.WebApi.Controllers;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Pictures;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Tests.Controllers
{
    public class PictureControllerTests
    {
        private readonly IPictureRepository _pictureRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly TestPictureGenerator _testPictureGenerator;
        private PictureController? _pictureController;

        public PictureControllerTests()
        {
            _pictureRepository = A.Fake<IPictureRepository>();
            _userRepository = A.Fake<IUserRepository>();
            _mapper = A.Fake<IMapper>();
            _testPictureGenerator = new TestPictureGenerator();
        }

        private PictureController GetControllerWithContext(string userId)
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

            return new PictureController(_pictureRepository,
                _userRepository,
                _mapper)
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async Task PictureController_AddPicture_ReturnsOK()
        {
            var userId = "1";
            _pictureController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var addPictureRequest = new AddPictureRequest()
            {
                Picture = _testPictureGenerator.GetPicture(),
                PictureDescription = "description"
            };
            var picture = addPictureRequest.Picture;
            var description = addPictureRequest.PictureDescription;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _pictureRepository.AddPicture(picture, userId, description)).Returns(true);

            var result = await _pictureController.AddPicture(addPictureRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PictureController_RemovePicture_ReturnsOK()
        {
            var userId = "1";
            _pictureController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var picture = A.Fake<Picture>();
            picture.UserId = userId;
            var removePictureRequest = new RemovePictureRequest()
            {
                PictureId = "1"
            };
            var pictureId = removePictureRequest.PictureId;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _pictureRepository.GetPictureTracking(pictureId)).Returns(picture);
            A.CallTo(() => _pictureRepository.RemovePicture(picture)).Returns(true);

            var result = await _pictureController.RemovePicture(removePictureRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PictureController_UpdatePictureDescription_ReturnsOK()
        {
            var userId = "1";
            _pictureController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var picture = A.Fake<Picture>();
            picture.UserId = userId;
            var updatePictureRequest = new UpdatePictureRequest()
            {
                PictureId = "1",
                Description = "description"
            };
            var pictureId = updatePictureRequest.PictureId;
            var description = updatePictureRequest.Description;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _pictureRepository.GetPictureTracking(pictureId)).Returns(picture);
            A.CallTo(() => _pictureRepository.UpdatePictureDescription(picture, description)).Returns(true);

            var result = await _pictureController.UpdatePictureDescription(updatePictureRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
