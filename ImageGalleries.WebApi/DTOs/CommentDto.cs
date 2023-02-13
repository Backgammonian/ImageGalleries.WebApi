namespace ImageGalleries.WebApi.DTOs
{
    public class CommentDto
    {
        public string Id { get; set; } = string.Empty;
        public string PictureId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
    }
}
