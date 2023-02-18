using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class UpdatePictureRequest
    {
        [Required]
        public string PictureId { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
