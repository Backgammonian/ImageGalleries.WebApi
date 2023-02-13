using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Models
{
    public class Tag
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public ICollection<PictureTag>? PictureTags { get; set; }
    }
}
