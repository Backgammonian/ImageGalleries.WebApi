using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.RefreshTokens;
using ImageGalleries.WebApi.Requests;
using ImageGalleries.WebApi.Responses;
using ImageGalleries.WebApi.Services.Authenticators;
using ImageGalleries.WebApi.Services.RandomGenerators;
using ImageGalleries.WebApi.Services.TokenValidators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly Authenticator _authenticator;
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRandomGenerator _randomGenerator;

        public AccountController(UserManager<User> userManager,
            Authenticator authenticator,
            RefreshTokenValidator refreshTokenValidator,
            IRefreshTokenRepository refreshTokenRepository,
            IRandomGenerator randomGenerator)
        {
            _userManager = userManager;
            _authenticator = authenticator;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
            _randomGenerator = randomGenerator;
        }

        private IActionResult GetBadRequestModelState()
        {
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

            return BadRequest(new ErrorResponse(errorMessages));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return GetBadRequestModelState();
            }

            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return BadRequest(new ErrorResponse("Password does not match confirm password."));
            }

            var registrationUser = new User()
            {
                Id = _randomGenerator.GetRandomId(),
                Email = registerRequest.Email,
                EmailConfirmed = true,
                UserName = registerRequest.Username,
                RegisterDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(registrationUser, registerRequest.Password);
            if (!result.Succeeded)
            {
                var errorDescriber = new IdentityErrorDescriber();
                var primaryError = result.Errors.FirstOrDefault();

                switch (primaryError?.Code)
                {
                    case nameof(errorDescriber.DuplicateEmail):
                        return Conflict(new ErrorResponse("Email already exists."));

                    case nameof(errorDescriber.DuplicateUserName):
                        return Conflict(new ErrorResponse("Username already exists."));
                }
            }

            await _userManager.AddToRoleAsync(registrationUser, Roles.UserRole);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return GetBadRequestModelState();
            }

            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return Unauthorized();
            }

            bool isCorrectPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isCorrectPassword)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            var response = await _authenticator.Authenticate(user, role);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return GetBadRequestModelState();
            }

            bool isValidRefreshToken = _refreshTokenValidator.Validate(refreshRequest.RefreshToken);
            if (!isValidRefreshToken)
            {
                return BadRequest(new ErrorResponse("Invalid refresh token."));
            }

            var refreshTokenDTO = await _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken);
            if (refreshTokenDTO == null)
            {
                return NotFound(new ErrorResponse("Invalid refresh token."));
            }

            await _refreshTokenRepository.Delete(refreshTokenDTO.Id);

            var user = await _userManager.FindByIdAsync(refreshTokenDTO.UserId.ToString());
            if (user == null)
            {
                return NotFound(new ErrorResponse("User not found."));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            var response = await _authenticator.Authenticate(user, role);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            if (!ModelState.IsValid)
            {
                return GetBadRequestModelState();
            }

            if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmNewPassword)
            {
                return BadRequest("New password doesn't match");
            }

            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, changePasswordRequest.OldPassword);
            if (!passwordCheck)
            {
                return BadRequest("Old password doesn't match");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, changePasswordRequest.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest("Can't change the password!");
            }

            return Ok();
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.User.FindFirstValue("UserId");

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            await _refreshTokenRepository.DeleteAll(userId);

            return NoContent();
        }
    }
}
