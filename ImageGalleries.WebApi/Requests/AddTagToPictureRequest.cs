using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class AddTagToPictureRequest
    {
        [Required]
        public string TagName { get; set; } = string.Empty;

        [Required]
        public string PictureId { get; set; } = string.Empty;
    }
}
