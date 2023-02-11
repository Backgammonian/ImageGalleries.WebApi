namespace ImageGalleries.WebApi.Models
{
    public class Score
    {
        public string PictureId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public Picture? Picture { get; set; }
        public User? User { get; set; }
        public int Amount { get; set; }
    }
}
