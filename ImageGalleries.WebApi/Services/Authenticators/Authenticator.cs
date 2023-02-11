using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.RefreshTokens;
using ImageGalleries.WebApi.Responses;
using ImageGalleries.WebApi.Services.RandomGenerators;
using ImageGalleries.WebApi.Services.TokenGenerators;

namespace ImageGalleries.WebApi.Services.Authenticators
{
    public class Authenticator
    {
        private readonly AccessTokenGenerator _accessTokenGenerator;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRandomGenerator _randomGenerator;

        public Authenticator(AccessTokenGenerator accessTokenGenerator,
            RefreshTokenGenerator refreshTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository,
            IRandomGenerator randomGenerator)
        {
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
            _randomGenerator = randomGenerator;
        }

        public async Task<AuthenticatedUserResponse> Authenticate(User user, string? role)
        {
            role = string.IsNullOrWhiteSpace(role) ? Roles.UserRole : role; 

            var accessToken = _accessTokenGenerator.GenerateToken(user, role);
            var refreshToken = _refreshTokenGenerator.GenerateToken();

            var refreshTokenDTO = new RefreshToken()
            {
                Id = _randomGenerator.GetRandomId(),
                Token = refreshToken,
                UserId = user.Id
            };
            await _refreshTokenRepository.Create(refreshTokenDTO);

            return new AuthenticatedUserResponse()
            {
                AccessToken = accessToken.Value,
                AccessTokenExpirationTime = accessToken.ExpirationTime,
                RefreshToken = refreshToken
            };
        }
    }
}
