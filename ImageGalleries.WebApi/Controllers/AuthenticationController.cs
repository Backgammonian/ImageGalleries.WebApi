using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Interfaces;
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
    [Route("auth")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userRepository;
        private readonly Authenticator _authenticator;
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRandomGenerator _randomGenerator;

        public AuthenticationController(UserManager<User> userRepository,
            Authenticator authenticator,
            RefreshTokenValidator refreshTokenValidator,
            IRefreshTokenRepository refreshTokenRepository,
            IRandomGenerator randomGenerator)
        {
            _userRepository = userRepository;
            _authenticator = authenticator;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
            _randomGenerator = randomGenerator;
        }

        private IActionResult BadRequestModelState()
        {
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

            return BadRequest(new ErrorResponse(errorMessages));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return BadRequest(new ErrorResponse("Password does not match confirm password."));
            }

            var registrationUser = new User()
            {
                Id = _randomGenerator.GetRandomId(),
                Email = registerRequest.Email,
                UserName = registerRequest.Username
            };

            var result = await _userRepository.CreateAsync(registrationUser, registerRequest.Password);
            if (!result.Succeeded)
            {
                var errorDescriber = new IdentityErrorDescriber();
                var primaryError = result.Errors.FirstOrDefault();

                if (primaryError?.Code == nameof(errorDescriber.DuplicateEmail))
                {
                    return Conflict(new ErrorResponse("Email already exists."));
                }
                else 
                if (primaryError?.Code == nameof(errorDescriber.DuplicateUserName))
                {
                    return Conflict(new ErrorResponse("Username already exists."));
                }
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var user = await _userRepository.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return Unauthorized();
            }

            bool isCorrectPassword = await _userRepository.CheckPasswordAsync(user, loginRequest.Password);
            if (!isCorrectPassword)
            {
                return Unauthorized();
            }

            var response = await _authenticator.Authenticate(user);

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
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

            var user = await _userRepository.FindByIdAsync(refreshTokenDTO.UserId.ToString());
            if (user == null)
            {
                return NotFound(new ErrorResponse("User not found."));
            }

            var response = await _authenticator.Authenticate(user);

            return Ok(response);
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
