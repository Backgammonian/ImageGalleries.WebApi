using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class CreateTagRequest
    {
        [Required]
        public string TagName { get; set; } = string.Empty;

        [Required]
        public string TagDescription { get; set; } = string.Empty;
    }
}
