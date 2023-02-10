using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Services.TokenGenerators
{
    public class RefreshTokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;
        private readonly TokenGenerator _tokenGenerator;

        public RefreshTokenGenerator(AuthenticationConfiguration configuration,
            TokenGenerator tokenGenerator)
        {
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
        }

        public string GenerateToken()
        {
            var expirationTime = DateTime.UtcNow
                .AddMinutes(_configuration.RefreshTokenExpirationMinutes);

            return _tokenGenerator.GenerateToken(_configuration.RefreshTokenSecret,
                _configuration.Issuer,
                _configuration.Audience,
                expirationTime);
        }
    }
}
