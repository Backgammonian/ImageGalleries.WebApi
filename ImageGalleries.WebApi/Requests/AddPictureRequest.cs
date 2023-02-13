using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class AddPictureRequest
    {
        [Required]
        public IFormFile? Picture { get; set; }

        [Required]
        public string PictureDescription { get; set; } = string.Empty;
    }
}
