using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class AddCommentRequest
    {
        [Required]
        public string PictureId { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
