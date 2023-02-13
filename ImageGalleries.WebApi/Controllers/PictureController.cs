using AutoMapper;
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
            var pictureDto = _mapper.Map<PictureDto>(picture);

            return Ok(pictureDto);
        }

        [HttpGet("uploader/{pictureId}")]
        public async Task<IActionResult> GetUploaderOfPicture(string pictureId)
        {
            var uploader = await _pictureRepository.GetUploaderOfPicture(pictureId);
            var uploaderDto = _mapper.Map<UserDto>(uploader);

            return Ok(uploaderDto);
        }

        [HttpGet("comments/{pictureId}")]
        public async Task<IActionResult> GetCommentsOfPicture(string pictureId)
        {
            var comments = await _pictureRepository.GetCommentsOfPicture(pictureId);
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
            var tagsDto = _mapper.Map<ICollection<TagDto>>(tags);

            return Ok(tagsDto);
        }

        [HttpGet("galleries/{pictureId}")]
        public async Task<IActionResult> GetGalleriesThatContainPicture(string pictureId)
        {
            var galleries = await _pictureRepository.GetGalleriesThatContainPicture(pictureId);
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

            var picture = addPictureRequest.Picture;

            if (picture == null ||
                picture.Length == 0)
            {
                return BadRequest("No picture!");
            }

            var added = await _pictureRepository.AddPicture(picture, userId, addPictureRequest.PictureDescription);
            if (added)
            {
                return Ok("Picture added successfully");
            }

            return BadRequest("Can't add the picture");
        }

        [Authorize]
        [HttpDelete("remove")]
        public async Task<IActionResult> RemovePicture([FromBody] string pictureId)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var picture = await _pictureRepository.GetPicture(pictureId);
            if (picture == null)
            {
                return BadRequest("Picture doesn't exist");
            }

            if (picture.UserId != userId)
            {
                return Unauthorized("No access for this operation");
            }

            var removed = await _pictureRepository.RemovePicture(pictureId);
            if (removed)
            {
                return Ok("Picture removed successfully");
            }

            return BadRequest("Can't remove the picture");
        }

        [Authorize]
        [HttpPut("update-description")]
        public async Task<IActionResult> UpdatePictureDescription([FromBody] string pictureId, string description)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var picture = await _pictureRepository.GetPicture(pictureId);
            if (picture == null)
            {
                return BadRequest("Picture doesn't exist");
            }

            if (picture.UserId != userId)
            {
                return Unauthorized("No access for this operation");
            }

            var updated = await _pictureRepository.UpdatePictureDescription(pictureId, description);
            if (updated)
            {
                return Ok("Picture's description updated successfully");
            }

            return BadRequest("Can't update the picture's description");
        }
    }
}
