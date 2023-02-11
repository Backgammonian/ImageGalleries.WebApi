using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Models
{
    public class ProfilePicture
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime UploadTime { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
