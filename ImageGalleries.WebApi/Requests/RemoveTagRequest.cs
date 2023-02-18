using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Requests
{
    public class RemoveTagRequest
    {
        [Required]
        public string TagName { get; set; } = string.Empty;
    }
}
