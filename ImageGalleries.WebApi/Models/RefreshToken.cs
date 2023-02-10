using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Models
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
