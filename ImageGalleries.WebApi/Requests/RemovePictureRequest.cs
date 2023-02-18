using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class RemovePictureRequest
    {
        [Required]
        public string PictureId { get; set; } = string.Empty;
    }
}
