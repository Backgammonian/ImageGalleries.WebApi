using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Models
{
    public class Picture
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime UploadTime { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
        public string Description { get; set; } = string.Empty;
        public ICollection<PreviewPicture>? Previews { get; set; }
        public ICollection<PictureGallery>? PictureGalleries { get; set; }
        public ICollection<PictureTag>? PictureTags { get; set; }
        public ICollection<Score>? Scores { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
