using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class UpdateGalleryRequest
    {
        [Required]
        public string GalleryId { get; set; } = string.Empty;

        [Required]
        public string NewName { get; set; } = string.Empty;

        [Required]
        public string NewDescription { get; set; } = string.Empty;
    }
}
