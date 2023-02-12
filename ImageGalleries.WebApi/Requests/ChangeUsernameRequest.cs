using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class ChangeUsernameRequest
    {
        [Required]
        public string NewUsername { get; set; } = string.Empty;
    }
}
