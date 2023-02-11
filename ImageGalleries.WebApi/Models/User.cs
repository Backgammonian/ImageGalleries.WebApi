using Microsoft.AspNetCore.Identity;

namespace ImageGalleries.WebApi.Models
{
    public class User : IdentityUser
    {
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<ProfilePicture>? ProfilePictures { get; set; }
        public ICollection<Picture>? Pictures { get; set; }
        public ICollection<Gallery>? Galleries { get; set; }
        public ICollection<Score>? Scores { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}