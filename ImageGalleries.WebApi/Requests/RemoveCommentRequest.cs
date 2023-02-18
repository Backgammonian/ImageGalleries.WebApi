using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class RemoveCommentRequest
    {
        [Required]
        public string CommentId { get; set; } = string.Empty;
    }
}
