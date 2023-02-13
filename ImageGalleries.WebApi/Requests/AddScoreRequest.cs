using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class AddScoreRequest
    {
        [Required]
        public string PictureId { get; set; } = string.Empty;

        [Required]
        public bool IsUpvote { get; set; }
    }
}
