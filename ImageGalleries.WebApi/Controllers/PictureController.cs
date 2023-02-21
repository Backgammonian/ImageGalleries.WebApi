using AutoMapper;
using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.DTOs;
using ImageGalleries.WebApi.Repositories.Pictures;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Controllers
{
    [ApiController]
    [Route("api/picture")]
    public class PictureController : Controller
    {
        private readonly IPictureRepository _pictureRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PictureController(IPictureRepository pictureRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _pictureRepository = pictureRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetPictures()
        {
            var pictures = await _pictureRepository.GetPictures();
            var picturesDto = _mapper.Map<ICollection<PictureDto>>(pictures);

            return Ok(picturesDto);
        }

        [HttpGet("{pictureId}")]
        public async Task<IActionResult> GetPicture(string pictureId)
        {
            var picture = await _pictureRepository.GetPicture(pictureId);
            if (picture == null)
            {
                return BadRequest("No picture");
            }

            var pictureDto = _mapper.Map<PictureDto>(picture);

            return Ok(pictureDto);
        }

        [HttpGet("uploader/{pictureId}")]
        public async Task<IActionResult> GetUploaderOfPicture(string pictureId)
        {
            var uploader = await _pictureRepository.GetUploaderOfPicture(pictureId);
            if (uploader == null)
            {
                return BadRequest("No picture");
            }

            var uploaderDto = _mapper.Map<UserDto>(uploader);

            return Ok(uploaderDto);
        }

        [HttpGet("comments/{pictureId}")]
        public async Task<IActionResult> GetCommentsOfPicture(string pictureId)
        {
            var comments = await _pictureRepository.GetCommentsOfPicture(pictureId);
            if (comments == null)
            {
                return BadRequest("No picture");
            }

            var commentsDto = _mapper.Map<ICollection<CommentDto>>(comments);

            return Ok(commentsDto);
        }

        [HttpGet("score/{pictureId}")]
        public async Task<IActionResult> GetScoreOfPicture(string pictureId)
        {
            var score = await _pictureRepository.GetScoreOfPicture(pictureId);

            return Ok(score);
        }

        [HttpGet("tags/{pictureId}")]
        public async Task<IActionResult> GetTagsOfPicture(string pictureId)
        {
            var tags = await _pictureRepository.GetTagsOfPicture(pictureId);
            if (tags == null)
            {
                return BadRequest("No picture");
            }

            var tagsDto = _mapper.Map<ICollection<TagDto>>(tags);

            return Ok(tagsDto);
        }

        [HttpGet("galleries/{pictureId}")]
        public async Task<IActionResult> GetGalleriesThatContainPicture(string pictureId)
        {
            var galleries = await _pictureRepository.GetGalleriesThatContainPicture(pictureId);
            if (galleries == null)
            {
                return BadRequest("No picture");
            }

            var galleriesDto = _mapper.Map<ICollection<GalleryDto>>(galleries);

            return Ok(galleriesDto);
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddPicture([FromForm] AddPictureRequest addPictureRequest)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var file = addPictureRequest.Picture;
            var description = addPictureRequest.PictureDescription;

            if (file == null ||
                file.Length == 0)
            {
                return BadRequest("No picture!");
            }

            var added = await _pictureRepository.AddPicture(file,
                userId,
                description);

            if (added)
            {
                return Ok("Picture added successfully");
            }

            return BadRequest("Can't add the picture");
        }

        [Authorize]
        [HttpDelete("remove")]
        public async Task<IActionResult> RemovePicture([FromBody] RemovePictureRequest request)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var pictureId = request.PictureId;

            var picture = await _pictureRepository.GetPictureTracking(pictureId);
            if (picture == null)
            {
                return BadRequest("Picture doesn't exist");
            }

            if (picture.UserId != userId &&
                HttpContext.User.FindFirstValue(ClaimTypes.Role) != Roles.AdminRole)
            {
                return Unauthorized("You can't remove this picture");
            }

            var removed = await _pictureRepository.RemovePicture(picture);
            if (removed)
            {
                return Ok("Picture removed successfully");
            }

            return BadRequest("Unknown problem occured while removing the picture");
        }

        [Authorize]
        [HttpPut("update-description")]
        public async Task<IActionResult> UpdatePictureDescription([FromBody] UpdatePictureRequest request)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var pictureId = request.PictureId;
            var description = request.Description;

            var picture = await _pictureRepository.GetPictureTracking(pictureId);
            if (picture == null)
            {
                return BadRequest("Picture doesn't exist");
            }

            if (picture.UserId != userId &&
                HttpContext.User.FindFirstValue(ClaimTypes.Role) != Roles.AdminRole)
            {
                return Unauthorized("You can't update this picture's description");
            }

            var updated = await _pictureRepository.UpdatePictureDescription(picture, description);
            if (updated)
            {
                return Ok("Picture's description updated successfully");
            }

            return BadRequest("Unknown problem occured while updating the picture's description");
        }
    }
}
