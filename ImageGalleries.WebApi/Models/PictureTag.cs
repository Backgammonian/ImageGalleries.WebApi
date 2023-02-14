namespace ImageGalleries.WebApi.Models
{
    public class PictureTag
    {
        public string PictureId { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public Picture? Picture { get; set; }
        public Tag? Tag { get; set; }
    }
}
