namespace ImageGalleries.WebApi.DTOs
{
    public class TagDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
    }
}
