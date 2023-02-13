using AutoMapper;
using ImageGalleries.WebApi.DTOs;
using ImageGalleries.WebApi.Repositories.Galleries;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Controllers
{
    [ApiController]
    [Route("api/gallery")]
    public class GalleyController : Controller
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GalleyController(IGalleryRepository galleryRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _galleryRepository = galleryRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetGalleries()
        {
            var galleries = await _galleryRepository.GetGalleries();
            var galleriesDto = _mapper.Map<ICollection<GalleryDto>>(galleries);

            return Ok(galleriesDto);
        }

        [HttpGet("{galleryId}")]
        public async Task<IActionResult> GetGallery(string galleryId)
        {
            var gallery = await _galleryRepository.GetGallery(galleryId);
            var galleryDto = _mapper.Map<GalleryDto>(gallery);

            return Ok(galleryDto);
        }

        [HttpGet("owner/{galleryId}")]
        public async Task<IActionResult> GetGalleryOwner(string galleryId)
        {
            var user = await _galleryRepository.GetGalleryOwner(galleryId);
            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        [HttpGet("pictures/{galleryId}")]
        public async Task<IActionResult> GetPicturesFromGallery(string galleryId)
        {
            var pictures = await _galleryRepository.GetPicturesFromGallery(galleryId);
            var picturesDto = _mapper.Map<ICollection<PictureDto>>(pictures);

            return Ok(picturesDto);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateGallery([FromBody] CreateGalleryRequest createGalleryRequest)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var created = await _galleryRepository.CreateGallery(userId, createGalleryRequest.GalleryName, createGalleryRequest.GalleryDescription);
            if (created)
            {
                return Ok("Gallery created successfully");
            }

            return BadRequest("Can't create gallery");
        }

        [Authorize]
        [HttpPost("add-image")]
        public async Task<IActionResult> AddPictureToGallery([FromBody] string galleryId, string pictureId)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var added = await _galleryRepository.AddPictureToGallery(userId, galleryId, pictureId);
            if (added)
            {
                return Ok("Picture added to gallery successfully");
            }

            return BadRequest("Can't add picture to gallery");
        }

        [Authorize]
        [HttpDelete("remove-image")]
        public async Task<IActionResult> RemovePictureFromGallery([FromBody] string galleryId, string pictureId)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var removed = await _galleryRepository.RemovePictureFromGallery(userId, galleryId, pictureId);
            if (removed)
            {
                return Ok("Picture removed from gallery successfully");
            }

            return BadRequest("Can't remove picture from gallery");
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateGalleryNameAndDescription([FromBody] string galleryId, string newName, string newDescription)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var updated = await _galleryRepository.UpdateGalleryNameAndDescription(userId, galleryId, newName, newDescription);
            if (updated)
            {
                return Ok("Gallery updated successfully");
            }

            return BadRequest("Can't update gallery");
        }

        [Authorize]
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveGallery([FromBody] string galleryId)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var removed = await _galleryRepository.RemoveGallery(userId, galleryId);
            if (removed)
            {
                return Ok("Gallery removed successfully");
            }

            return BadRequest("Can't remove gallery");
        }
    }
}
