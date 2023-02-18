using AutoMapper;
using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.DTOs;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Pictures;
using ImageGalleries.WebApi.Repositories.Tags;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Controllers
{
    [ApiController]
    [Route("api/tag")]
    public class TagController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPictureRepository _pictureRepository;
        private readonly IMapper _mapper;

        public TagController(ITagRepository tagRepository,
            IUserRepository userRepository,
            IPictureRepository pictureRepository,
            IMapper mapper)
        {
            _tagRepository = tagRepository;
            _userRepository = userRepository;
            _pictureRepository = pictureRepository;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagRepository.GetTags();
            var tagsDto = _mapper.Map<ICollection<TagDto>>(tags);

            return Ok(tagsDto);
        }

        [HttpGet("{tagName}")]
        public async Task<IActionResult> GetTag(string tagName)
        {
            var tag = await _tagRepository.GetTag(tagName);
            if (tag == null)
            {
                return BadRequest("No tag");
            }

            var tagDto = _mapper.Map<TagDto>(tag);

            return Ok(tagDto);
        }

        [HttpGet("pictures/{tagName}")]
        public async Task<IActionResult> GetPicturesByTag(string tagName)
        {
            var pictures = await _tagRepository.GetPicturesByTag(tagName);
            if (pictures == null)
            {
                return BadRequest("No tag");
            }

            var picturesDto = _mapper.Map<ICollection<PictureDto>>(pictures);

            return Ok(picturesDto);
        }

        [Authorize(Roles = Roles.AdminRole)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest createTagRequest)
        {
            var created = await _tagRepository.CreateTag(createTagRequest.TagName,
                createTagRequest.TagDescription);

            if (created)
            {
                return Ok("Tag created successfully");
            }

            return BadRequest("Can't create the tag");
        }

        [Authorize(Roles = Roles.AdminRole)]
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveTag([FromBody] RemoveTagRequest request)
        {
            var tagName = request.TagName;

            var tag = await _tagRepository.GetTag(tagName);
            if (tag == null)
            {
                return BadRequest("Tag doesn't exist");
            }

            var removed = await _tagRepository.RemoveTag(tag);
            if (removed)
            {
                return Ok("Tag removed successfully");
            }

            return BadRequest("Can't remove the tag");
        }

        [Authorize(Roles = Roles.AdminRole)]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTag([FromBody] UpdateTagRequest updateTagRequest)
        {
            var tag = await _tagRepository.GetTag(updateTagRequest.TagName);
            if (tag == null)
            {
                return BadRequest("Tag doesn't exist");
            }

            var updated = await _tagRepository.UpdateTag(tag,
                updateTagRequest.NewTagName,
                updateTagRequest.NewTagDescription);

            if (updated)
            {
                return Ok("Tag updated successfully");
            }

            return BadRequest("Can't update the tag");
        }

        [Authorize]
        [HttpPost("add-tag-to-picture")]
        public async Task<IActionResult> AddTagToPicture([FromBody] AddTagToPictureRequest request)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var tagName = request.TagName;
            var pictureId = request.PictureId;

            var tag = await _tagRepository.GetTag(tagName);
            if (tag == null)
            {
                return BadRequest("Tag doesn't exist");
            }

            var picture = await _pictureRepository.GetPicture(pictureId);
            if (picture == null)
            {
                return BadRequest("Picture doesn't exist");
            }

            if (picture.UserId != userId &&
                HttpContext.User.FindFirstValue(ClaimTypes.Role) != Roles.AdminRole)
            {
                return Unauthorized("You can't add tags to this picture");
            }

            var added = await _tagRepository.AddTagToPicture(tag, picture);
            if (added)
            {
                return Ok("Tag added to the picture successfully");
            }

            return BadRequest("Unknown problem occured while adding tag to the picture");
        }

        [Authorize]
        [HttpDelete("remove-tag-from-picture")]
        public async Task<IActionResult> RemoveTagFromPicture([FromBody] RemoveTagFromPictureRequest request)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var tagName = request.TagName;
            var pictureId = request.PictureId;

            var tag = await _tagRepository.GetTag(tagName);
            if (tag == null)
            {
                return BadRequest("Tag doesn't exist");
            }

            var picture = await _pictureRepository.GetPicture(pictureId);
            if (picture == null)
            {
                return BadRequest("Picture doesn't exist");
            }

            if (picture.UserId != userId &&
                HttpContext.User.FindFirstValue(ClaimTypes.Role) != Roles.AdminRole)
            {
                return Unauthorized("You can't remove tags from this picture");
            }

            var removed = await _tagRepository.RemoveTagFromPicture(tag, picture);
            if (removed)
            {
                return Ok("Tag removed from the picture successfully");
            }

            return BadRequest("Unknown problem occured while removing tag from the picture");
        }
    }
}
