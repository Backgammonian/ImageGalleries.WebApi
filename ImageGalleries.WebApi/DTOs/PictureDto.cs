namespace ImageGalleries.WebApi.DTOs
{
    public class PictureDto
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string PreviewUrl { get; set; } = string.Empty;
        public DateTime UploadTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
