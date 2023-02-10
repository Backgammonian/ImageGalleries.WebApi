namespace ImageGalleries.WebApi.Models
{
    public class AccessToken
    {
        public string Value { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
    }
}
