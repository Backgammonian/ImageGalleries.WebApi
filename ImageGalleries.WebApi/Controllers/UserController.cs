using AutoMapper;
using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.DTOs;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return BadRequest("No user");
            }

            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        [HttpGet("pictures/{userId}")]
        public async Task<IActionResult> GetPicturesOfUser(string userId)
        {
            var pictures = await _userRepository.GetPicturesOfUser(userId);
            if (pictures == null)
            {
                return BadRequest("No user");
            }

            var picturesDto = _mapper.Map<ICollection<PictureDto>>(pictures);

            return Ok(picturesDto);
        }

        [HttpGet("galleries/{userId}")]
        public async Task<IActionResult> GetGalleriesOfUser(string userId)
        {
            var galleries = await _userRepository.GetGalleriesOfUser(userId);
            if (galleries == null)
            {
                return BadRequest("No user");
            }

            var galleriesDto = _mapper.Map<ICollection<GalleryDto>>(galleries);

            return Ok(galleriesDto);
        }

        [HttpGet("scores/{userId}")]
        public async Task<IActionResult> GetScoresOfUser(string userId)
        {
            var scores = await _userRepository.GetScoresOfUser(userId);
            if (scores == null)
            {
                return BadRequest("No user");
            }

            var scoresDto = _mapper.Map<ICollection<ScoreDto>>(scores);

            return Ok(scoresDto);
        }

        [HttpGet("comments/{userId}")]
        public async Task<IActionResult> GetCommentsOfUser(string userId)
        {
            var comments = await _userRepository.GetCommentsOfUser(userId);
            if (comments == null)
            {
                return BadRequest("No user");
            }

            var commentsDto = _mapper.Map<ICollection<CommentDto>>(comments);

            return Ok(commentsDto);
        }

        [Authorize]
        [HttpPut("update-profile-picture")]
        public async Task<IActionResult> ChangeProfilePicture([FromForm] ChangeProfilePictureRequest changeProfilePictureRequest)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var profilePicture = changeProfilePictureRequest.ProfilePicture;

            if (profilePicture == null ||
                profilePicture.Length == 0)
            {
                return BadRequest("No image!");
            }

            var updated = await _userRepository.UpdateProfilePicture(user, profilePicture);
            if (updated)
            {
                return Ok("Profile picture updated successfully");
            }

            return BadRequest("Can't update profile image");
        }

        [Authorize]
        [HttpPut("update-username")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameRequest changeUsernameRequest)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var newUsername = changeUsernameRequest.NewUsername;
            var updated = await _userRepository.UpdateUsername(user, newUsername);
            if (updated)
            {
                return Ok("Username updated successfully");
            }

            return BadRequest("Can't update username");
        }

        [Authorize]
        [HttpPost("add-comment")]
        public async Task<IActionResult> AddCommentToPicture([FromBody] AddCommentRequest addCommentRequest)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var added = await _userRepository.AddCommentToPicture(userId,
                addCommentRequest.PictureId,
                addCommentRequest.Content);

            if (added)
            {
                return Ok("Comment added successfully");
            }

            return BadRequest("Can't add the comment");
        }

        [Authorize]
        [HttpDelete("remove-comment")]
        public async Task<IActionResult> RemoveComment([FromBody] RemoveCommentRequest request)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var commentId = request.CommentId;

            var comment = await _userRepository.GetComment(commentId);
            if (comment == null) 
            {
                return BadRequest("Comment doesn't exist");
            }

            if (comment.UserId != userId &&
                HttpContext.User.FindFirstValue(ClaimTypes.Role) != Roles.AdminRole) 
            {
                return Unauthorized("You can't remove this comment");
            }

            var removed = await _userRepository.RemoveComment(comment);
            if (removed)
            {
                return Ok("Comment removed successfully");
            }

            return BadRequest("Unknown problem occured while removing the comment");
        }

        [Authorize]
        [HttpPost("add-score")]
        public async Task<IActionResult> AddScoreToPicture([FromBody] AddScoreRequest addScoreRequest)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var score = await _userRepository.GetScore(userId, addScoreRequest.PictureId);
            if (score != null)
            {
                await _userRepository.RemoveScoreFromPicture(score);
            }

            var added = await _userRepository.AddScoreToPicture(userId,
                addScoreRequest.PictureId,
                addScoreRequest.IsUpvote ? 1 : -1);

            if (added)
            {
                return Ok("Score added successfully");
            }

            return BadRequest("Unknown problem occured while adding the score");
        }
    }
}
