using AutoMapper;
using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.DTOs;
using ImageGalleries.WebApi.Models;
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
    public class GalleryController : Controller
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GalleryController(IGalleryRepository galleryRepository,
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
            if (gallery == null)
            {
                return BadRequest("No gallery");
            }

            var galleryDto = _mapper.Map<GalleryDto>(gallery);

            return Ok(galleryDto);
        }

        [HttpGet("owner/{galleryId}")]
        public async Task<IActionResult> GetGalleryOwner(string galleryId)
        {
            var user = await _galleryRepository.GetGalleryOwner(galleryId);
            if (user == null)
            {
                return BadRequest("No gallery");
            }

            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        [HttpGet("pictures/{galleryId}")]
        public async Task<IActionResult> GetPicturesFromGallery(string galleryId)
        {
            var pictures = await _galleryRepository.GetPicturesFromGallery(galleryId);
            if (pictures == null)
            {
                return BadRequest("No gallery");
            }

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

            var created = await _galleryRepository.CreateGallery(userId,
                createGalleryRequest.GalleryName,
                createGalleryRequest.GalleryDescription);

            if (created)
            {
                return Ok("Gallery created successfully");
            }

            return BadRequest("Can't create gallery");
        }

        [Authorize]
        [HttpPost("add-image")]
        public async Task<IActionResult> AddPictureToGallery([FromBody] AddPictureToGalleryRequest request)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var galleryId = request.GalleryId;
            var pictureId = request.PictureId;

            var gallery = await _galleryRepository.GetGallery(galleryId);
            if (gallery == null)
            {
                return BadRequest("Gallery doesn't exist");
            }

            if (gallery.UserId != userId)
            {
                return Unauthorized("You can't add pictures to this gallery");
            }

            var added = await _galleryRepository.AddPictureToGallery(gallery, pictureId);
            if (added)
            {
                return Ok("Picture added to gallery successfully");
            }

            return BadRequest("Unknown problem occured while adding picture to the gallery");
        }

        [Authorize]
        [HttpDelete("remove-image")]
        public async Task<IActionResult> RemovePictureFromGallery(RemovePictureFromGalleryRequest request)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var galleryId = request.GalleryId;
            var pictureId = request.PictureId;

            var gallery = await _galleryRepository.GetGallery(galleryId);
            if (gallery == null)
            {
                return BadRequest("Gallery doesn't exist");
            }

            if (gallery.UserId != userId)
            {
                return Unauthorized("You can't remove pictures from this gallery");
            }

            var removed = await _galleryRepository.RemovePictureFromGallery(gallery, pictureId);
            if (removed)
            {
                return Ok("Picture removed from gallery successfully");
            }

            return BadRequest("Unknown problem occured while removing picture from the gallery");
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateGalleryNameAndDescription([FromBody] UpdateGalleryRequest request)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var galleryId = request.GalleryId;
            var newName = request.NewName;
            var newDescription = request.NewDescription;

            var gallery = await _galleryRepository.GetGalleryTracking(galleryId);
            if (gallery == null)
            {
                return BadRequest("Gallery doesn't exist");
            }

            if (gallery.UserId != userId &&
                HttpContext.User.FindFirstValue(ClaimTypes.Role) != Roles.AdminRole)
            {
                return Unauthorized("You can't update this gallery");
            }

            var updated = await _galleryRepository.UpdateGalleryNameAndDescription(gallery, newName, newDescription);
            if (updated)
            {
                return Ok("Gallery updated successfully");
            }

            return BadRequest("Unknown problem occured while updating the gallery");
        }

        [Authorize]
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveGallery([FromBody] RemoveGalleryRequest request)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var galleryId = request.GalleryId;

            var gallery = await _galleryRepository.GetGalleryTracking(galleryId);
            if (gallery == null)
            {
                return BadRequest("Gallery doesn't exist");
            }

            if (gallery.UserId != userId &&
                HttpContext.User.FindFirstValue(ClaimTypes.Role) != Roles.AdminRole)
            {
                return Unauthorized("You can't remove this gallery");
            }

            var removed = await _galleryRepository.RemoveGallery(gallery);
            if (removed)
            {
                return Ok("Gallery removed successfully");
            }

            return BadRequest("Unknown problem occured while removing the gallery");
        }
    }
}
