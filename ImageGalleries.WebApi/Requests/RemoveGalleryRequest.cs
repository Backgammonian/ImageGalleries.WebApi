using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class RemoveGalleryRequest
    {
        [Required]
        public string GalleryId { get; set; } = string.Empty;
    }
}
