using System.Net.Http;
using ImageGallery.API.Client.Service.Interface;

namespace ImageGallery.API.Client.Service.Services
{
    public class ImageGalleryCommandService : IImageGalleryCommandService
    {
        private readonly HttpClient _client;

        public ImageGalleryCommandService(HttpClient client)
        {
            _client = client;
        }
    }
}
 