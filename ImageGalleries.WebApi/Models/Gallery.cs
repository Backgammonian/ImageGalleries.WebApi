using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Models
{
    public class Gallery
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<PictureGallery>? PictureGalleries { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
