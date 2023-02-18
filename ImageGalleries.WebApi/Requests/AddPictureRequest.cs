using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class AddPictureRequest
    {
        [Required]
        public IFormFile? Picture { get; set; }

        public string PictureDescription { get; set; } = string.Empty;
    }
}
