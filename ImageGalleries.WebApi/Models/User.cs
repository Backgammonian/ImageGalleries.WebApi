using Microsoft.AspNetCore.Identity;

namespace ImageGalleries.WebApi.Models
{
    public class User : IdentityUser
    {
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<Picture>? Pictures { get; set; }
        public ICollection<Gallery>? Galleries { get; set; }
        public ICollection<Score>? Scores { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}