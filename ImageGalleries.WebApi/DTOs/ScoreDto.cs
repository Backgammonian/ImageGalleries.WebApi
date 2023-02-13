namespace ImageGalleries.WebApi.DTOs
{
    public class ScoreDto
    {
        public string Id { get; set; } = string.Empty;
        public string PictureId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int Amount { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
