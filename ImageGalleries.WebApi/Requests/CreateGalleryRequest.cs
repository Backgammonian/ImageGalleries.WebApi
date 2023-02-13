using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class CreateGalleryRequest
    {
        [Required]
        public string GalleryName { get; set; } = string.Empty;

        public string GalleryDescription { get; set; } = string.Empty;
    }
}
