using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Models
{
    public class PreviewPicture
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime UploadTime { get; set; }
        public string PictureId { get; set; } = string.Empty;
        public Picture? Picture { get; set; }
    }
}
