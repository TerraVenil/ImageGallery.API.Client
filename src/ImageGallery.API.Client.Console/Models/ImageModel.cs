using System;

namespace ImageGallery.API.Client.Console.Models
{
    public class ImageModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

        public string Category { get; set; }
    }
}
