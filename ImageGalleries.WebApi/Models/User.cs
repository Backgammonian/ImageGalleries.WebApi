using Microsoft.AspNetCore.Identity;

namespace ImageGalleries.WebApi.Models
{
    public class User : IdentityUser
    {
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}
