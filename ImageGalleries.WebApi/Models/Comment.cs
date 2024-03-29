﻿using System.ComponentModel.DataAnnotations;

namespace ImageGalleries.WebApi.Models
{
    public class Comment
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string PictureId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public Picture? Picture { get; set; }
        public User? User { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
    }
}
