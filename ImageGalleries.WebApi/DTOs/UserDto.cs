namespace ImageGalleries.WebApi.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime RegisterDate { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}
