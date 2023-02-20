using ImageGalleries.WebApi.Controllers;
using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.RefreshTokens;
using ImageGalleries.WebApi.Requests;
using ImageGalleries.WebApi.Responses;
using ImageGalleries.WebApi.Services.Authenticators;
using ImageGalleries.WebApi.Services.RandomGenerators;
using ImageGalleries.WebApi.Services.TokenValidators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticator _authenticator;
        private readonly IRefreshTokenValidator _refreshTokenValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRandomGenerator _randomGenerator;
        private AccountController? _accountController;

        public AccountControllerTests()
        {
            _userManager = A.Fake<UserManager<User>>();
            _authenticator = A.Fake<IAuthenticator>();
            _refreshTokenValidator = A.Fake<IRefreshTokenValidator>();
            _refreshTokenRepository = A.Fake<IRefreshTokenRepository>();
            _randomGenerator = A.Fake<IRandomGenerator>();
        }

        private AccountController GetController()
        {
            return new AccountController(_userManager,
                _authenticator,
                _refreshTokenValidator,
                _refreshTokenRepository,
                _randomGenerator);
        }

        private AccountController GetControllerWithContext(string userId)
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

            return new AccountController(_userManager,
                _authenticator,
                _refreshTokenValidator,
                _refreshTokenRepository,
                _randomGenerator)
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async Task AccountController_Register_ReturnsOK()
        {
            _accountController = GetController();

            var registerRequest = new RegisterRequest()
            {
                Email = "test-user@mail.com",
                Username = "username",
                Password = "123456",
                ConfirmPassword = "123456"
            };
            var registrationUser = A.Fake<User>();
            var registrationResult = IdentityResult.Success;
            A.CallTo(() => _userManager.CreateAsync(registrationUser, registerRequest.Password)).Returns(registrationResult);
            A.CallTo(() => _userManager.AddToRoleAsync(registrationUser, Roles.UserRole));

            var result = await _accountController.Register(registerRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task AccountController_Login_ReturnsOK()
        {
            _accountController = GetController();

            var loginRequest = A.Fake<LoginRequest>();
            var user = A.Fake<User>();
            var role = Roles.UserRole;
            var response = A.Fake<AuthenticatedUserResponse>();
            A.CallTo(() => _userManager.FindByEmailAsync(loginRequest.Email)).Returns(user);
            A.CallTo(() => _userManager.CheckPasswordAsync(user, loginRequest.Password)).Returns(true);
            A.CallTo(() => _authenticator.Authenticate(user, role)).Returns(response);

            var result = await _accountController.Login(loginRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AccountController_Refresh_ReturnsOK()
        {
            _accountController = GetController();

            var refreshRequest = A.Fake<RefreshRequest>();
            var refreshTokenDTO = A.Fake<RefreshToken>();
            var user = A.Fake<User>();
            var role = Roles.UserRole;
            var response = A.Fake<AuthenticatedUserResponse>();
            A.CallTo(() => _refreshTokenValidator.Validate(refreshRequest.RefreshToken)).Returns(true);
            A.CallTo(() => _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken)).Returns(refreshTokenDTO);
            A.CallTo(() => _refreshTokenRepository.Delete(refreshTokenDTO.Id));
            A.CallTo(() => _userManager.FindByIdAsync(refreshTokenDTO.UserId)).Returns(user);
            A.CallTo(() => _authenticator.Authenticate(user, role)).Returns(response);

            var result = await _accountController.Refresh(refreshRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AccountController_ChangePassword_ReturnsOK()
        {
            var userId = "1";
            _accountController = GetControllerWithContext(userId);

            var changePasswordRequest = new ChangePasswordRequest()
            {
                OldPassword = "123",
                NewPassword = "134",
                ConfirmNewPassword = "134"
            };
            var user = A.Fake<User>();
            var token = "someRandomToken";
            var resetPasswordResult = IdentityResult.Success;
            
            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _userManager.CheckPasswordAsync(user, changePasswordRequest.OldPassword)).Returns(true);
            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(user)).Returns(token);
            A.CallTo(() => _userManager.ResetPasswordAsync(user, token, changePasswordRequest.NewPassword)).Returns(resetPasswordResult);

            var result = await _accountController.ChangePassword(changePasswordRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task AccountController_Logout_ReturnsOK()
        {
            var userId = "1";
            _accountController = GetControllerWithContext(userId);

            A.CallTo(() => _refreshTokenRepository.DeleteAll(userId));

            var result = await _accountController.Logout();

            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
