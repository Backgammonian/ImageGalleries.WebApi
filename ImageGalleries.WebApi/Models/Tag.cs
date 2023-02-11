using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Models
{
    public class Tag
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ICollection<PictureTag>? PictureTags { get; set; }
    }
}
