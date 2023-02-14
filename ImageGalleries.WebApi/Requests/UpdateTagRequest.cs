using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class UpdateTagRequest
    {
        [Required]
        public string TagName { get; set; } = string.Empty;

        [Required]
        public string NewTagName { get; set; } = string.Empty;

        [Required]
        public string NewTagDescription { get; set; } = string.Empty;
    }
}
