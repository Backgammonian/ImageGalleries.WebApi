using AutoMapper;
using FakeItEasy;
using ImageGalleries.WebApi.Controllers;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Pictures;
using ImageGalleries.WebApi.Repositories.Tags;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Tests.Controllers
{
    public class TagControllerTests
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPictureRepository _pictureRepository;
        private readonly IMapper _mapper;
        private TagController? _tagController;

        public TagControllerTests()
        {
            _tagRepository = A.Fake<ITagRepository>();
            _userRepository = A.Fake<IUserRepository>();
            _pictureRepository = A.Fake<IPictureRepository>();
            _mapper = A.Fake<IMapper>();
        }

        private TagController GetController()
        {
            return new TagController(_tagRepository,
                _userRepository,
                _pictureRepository,
                _mapper);
        }

        private TagController GetControllerWithContext(string userId)
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

            return new TagController(_tagRepository,
                _userRepository,
                _pictureRepository,
                _mapper)
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async Task TagController_CreateTag_ReturnsOK()
        {
            _tagController = GetController();

            var createTagRequest = new CreateTagRequest()
            {
                TagName = "name",
                TagDescription = "description"
            };
            A.CallTo(() => _tagRepository.CreateTag(createTagRequest.TagName,
                createTagRequest.TagDescription)).Returns(true);

            var result = await _tagController.CreateTag(createTagRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task TagController_RemoveTag_ReturnsOK()
        {
            _tagController = GetController();

            var tag = A.Fake<Tag>();
            var removeTagRequest = new RemoveTagRequest()
            {
                TagName = "name"
            };
            var tagName = removeTagRequest.TagName;
            A.CallTo(() => _tagRepository.GetTag(tagName)).Returns(tag);
            A.CallTo(() => _tagRepository.RemoveTag(tag)).Returns(true);

            var result = await _tagController.RemoveTag(removeTagRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task TagController_UpdateTag_ReturnsOK()
        {
            _tagController = GetController();

            var tag = A.Fake<Tag>();
            var updateTagRequest = new UpdateTagRequest()
            {
                TagName = "name",
                NewTagName = "new-name",
                NewTagDescription = "new-description"
            };
            A.CallTo(() => _tagRepository.GetTag(updateTagRequest.TagName)).Returns(tag);
            A.CallTo(() => _tagRepository.UpdateTag(tag,
                updateTagRequest.NewTagName,
                updateTagRequest.NewTagDescription)).Returns(true);

            var result = await _tagController.UpdateTag(updateTagRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task TagController_AddTagToPicture_ReturnsOK()
        {
            var userId = "1";
            _tagController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var tag = A.Fake<Tag>();
            var picture = A.Fake<Picture>();
            picture.UserId = userId;
            var addTagToPictureRequest = new AddTagToPictureRequest()
            {
                TagName = "name",
                PictureId = "1"
            };
            var tagName = addTagToPictureRequest.TagName;
            var pictureId = addTagToPictureRequest.PictureId;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _tagRepository.GetTag(tagName)).Returns(tag);
            A.CallTo(() => _pictureRepository.GetPicture(pictureId)).Returns(picture);
            A.CallTo(() => _tagRepository.AddTagToPicture(tag, picture)).Returns(true);

            var result = await _tagController.AddTagToPicture(addTagToPictureRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task TagController_RemoveTagFromPicture_ReturnsOK()
        {
            var userId = "1";
            _tagController = GetControllerWithContext(userId);

            var user = A.Fake<User>();
            var tag = A.Fake<Tag>();
            var picture = A.Fake<Picture>();
            picture.UserId = userId;
            var removeTagFromPictureRequest = new RemoveTagFromPictureRequest()
            {
                TagName = "name",
                PictureId = "1"
            };
            var tagName = removeTagFromPictureRequest.TagName;
            var pictureId = removeTagFromPictureRequest.PictureId;
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _tagRepository.GetTag(tagName)).Returns(tag);
            A.CallTo(() => _pictureRepository.GetPicture(pictureId)).Returns(picture);
            A.CallTo(() => _tagRepository.RemoveTagFromPicture(tag, picture)).Returns(true);

            var result = await _tagController.RemoveTagFromPicture(removeTagFromPictureRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
