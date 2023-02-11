namespace ImageGalleries.WebApi.Models
{
    public class PictureGallery
    {
        public string PictureId { get; set; } = string.Empty;
        public string GalleryId { get; set; } = string.Empty;
        public Picture? Picture { get; set; }
        public Gallery? Gallery { get; set; }
    }
}