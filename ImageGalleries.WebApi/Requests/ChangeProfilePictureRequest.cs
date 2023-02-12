using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class ChangeProfilePictureRequest
    {
        [Required]
        public IFormFile? ProfilePicture { get; set; }
    }
}
