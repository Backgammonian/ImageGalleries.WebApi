using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class RemovePictureFromGalleryRequest
    {
        [Required]
        public string GalleryId { get; set; } = string.Empty;

        [Required]
        public string PictureId { get; set; } = string.Empty;
    }
}
