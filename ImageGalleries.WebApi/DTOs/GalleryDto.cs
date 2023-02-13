namespace ImageGalleries.WebApi.DTOs
{
    public class GalleryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
