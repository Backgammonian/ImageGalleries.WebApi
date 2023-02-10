using ImageGalleries.WebApi.Models;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Services.TokenGenerators
{
    public class AccessTokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;
        private readonly TokenGenerator _tokenGenerator;

        public AccessTokenGenerator(AuthenticationConfiguration configuration,
            TokenGenerator tokenGenerator)
        {
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
        }

        public AccessToken GenerateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim("UserId", user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var expirationTime = DateTime.UtcNow
                .AddMinutes(_configuration.AccessTokenExpirationMinutes);
            var value = _tokenGenerator.GenerateToken(_configuration.AccessTokenSecret,
                _configuration.Issuer,
                _configuration.Audience,
                expirationTime,
                claims);

            return new AccessToken()
            {
                Value = value,
                ExpirationTime = expirationTime
            };
        }
    }
}
