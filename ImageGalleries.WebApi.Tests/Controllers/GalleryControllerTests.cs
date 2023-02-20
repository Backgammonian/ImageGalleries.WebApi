using AutoMapper;
using FakeItEasy;
using ImageGalleries.WebApi.Controllers;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Galleries;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Tests.Controllers
{
    public class GalleryControllerTests
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private GalleryController? _galleryController;

        public GalleryControllerTests()
        {
            _galleryRepository = A.Fake<IGalleryRepository>();
            _userRepository = A.Fake<IUserRepository>();
            _mapper = A.Fake<IMapper>();
        }

        private GalleryController GetControllerWithContext(string userId)
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

            return new GalleryController(_galleryRepository,
                _userRepository,
                _mapper)
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async Task GalleryController_CreateGallery_ReturnsOK()
        {
            var userId = "1";
            _galleryController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var createGalleryRequest = new CreateGalleryRequest()
            {
                GalleryName = "name",
                GalleryDescription = "description"
            };
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _galleryRepository.CreateGallery(userId,
                createGalleryRequest.GalleryName,
                createGalleryRequest.GalleryDescription)).Returns(true);

            var result = await _galleryController.CreateGallery(createGalleryRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GalleryController_AddPictureToGallery_ReturnsOK()
        {
            var userId = "1";
            _galleryController = GetControllerWithContext(userId);

            var galleryId = "1";
            var pictureId = "1";
            var addPictureToGalleryRequest = new AddPictureToGalleryRequest()
            {
                GalleryId = galleryId,
                PictureId = pictureId
            };
            var user = A.Fake<User>();
            var gallery = A.Fake<Gallery>();
            gallery.UserId = userId;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _galleryRepository.GetGallery(galleryId)).Returns(gallery);
            A.CallTo(() => _galleryRepository.AddPictureToGallery(gallery, pictureId)).Returns(true);

            var result = await _galleryController.AddPictureToGallery(addPictureToGalleryRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GalleryController_RemovePictureFromGallery_ReturnsOK()
        {
            var userId = "1";
            _galleryController = GetControllerWithContext(userId);

            var galleryId = "1";
            var pictureId = "1";
            var removePictureFromGalleryRequest = new RemovePictureFromGalleryRequest()
            {
                GalleryId = galleryId,
                PictureId = pictureId
            };
            var user = A.Fake<User>();
            var gallery = A.Fake<Gallery>();
            gallery.UserId = userId;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _galleryRepository.GetGallery(galleryId)).Returns(gallery);
            A.CallTo(() => _galleryRepository.RemovePictureFromGallery(gallery, pictureId)).Returns(true);

            var result = await _galleryController.RemovePictureFromGallery(removePictureFromGalleryRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GalleryController_UpdateGalleryNameAndDescription_ReturnsOK()
        {
            var userId = "1";
            _galleryController = GetControllerWithContext(userId);

            var galleryId = "1";
            var updateGalleryRequest = new UpdateGalleryRequest()
            {
                GalleryId = galleryId,
                NewName = "name",
                NewDescription = "description"
            };
            var user = A.Fake<User>();
            var gallery = A.Fake<Gallery>();
            gallery.UserId = userId;
            var newName = updateGalleryRequest.NewName;
            var newDescription = updateGalleryRequest.NewDescription;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _galleryRepository.GetGallery(galleryId)).Returns(gallery);
            A.CallTo(() => _galleryRepository.UpdateGalleryNameAndDescription(gallery, newName, newDescription)).Returns(true);

            var result = await _galleryController.UpdateGalleryNameAndDescription(updateGalleryRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GalleryController_RemoveGallery_ReturnsOK()
        {
            var userId = "1";
            _galleryController = GetControllerWithContext(userId);

            var galleryId = "1";
            var removeGalleryRequest = new RemoveGalleryRequest()
            {
                GalleryId = galleryId
            };
            var user = A.Fake<User>();
            var gallery = A.Fake<Gallery>();
            gallery.UserId = userId;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _galleryRepository.GetGallery(galleryId)).Returns(gallery);
            A.CallTo(() => _galleryRepository.RemoveGallery(gallery)).Returns(true);

            var result = await _galleryController.RemoveGallery(removeGalleryRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
