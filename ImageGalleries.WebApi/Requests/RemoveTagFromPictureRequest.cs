using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class RemoveTagFromPictureRequest
    {
        [Required]
        public string TagName { get; set; } = string.Empty;

        [Required]
        public string PictureId { get; set; } = string.Empty;
    }
}
