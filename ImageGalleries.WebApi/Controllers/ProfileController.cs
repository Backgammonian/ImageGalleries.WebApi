using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Pictures;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Controllers
{
    [ApiController]
    [Route("profile")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IPictureRepository _pictureRepository;
        private readonly IUserRepository _userRepository;

        public ProfileController(UserManager<User> userManager,
            IPictureRepository pictureRepository,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _pictureRepository = pictureRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpPost("change-profile-picture")]
        public async Task<IActionResult> ChangeProfilePicture([FromForm] ChangeProfilePictureRequest changeProfilePictureRequest)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userManager.FindByIdAsync(userId);
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

            var updated = await _pictureRepository.UpdateProfilePicture(profilePicture, user);
            if (updated)
            {
                return Ok("Profile picture updated successfully");
            }

            return BadRequest("Can't update profile image");
        }

        [Authorize]
        [HttpPost("change-username")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameRequest changeUsernameRequest)
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userManager.FindByIdAsync(userId);
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
    }
}
