using System;

namespace ImageGallery.API.Client.Service.Models
{
    public class ImageModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

        public string Category { get; set; }

        public string PhotoId { get; set; }
    }
}
