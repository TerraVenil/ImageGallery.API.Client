using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ImageGallery.API.Client.Service.Services
{
    public class ImageGalleryCommandService : IImageGalleryCommandService
    {
        private readonly HttpClient _client;

        private readonly ImagegalleryApiConfiguration _settings;

        public ImageGalleryCommandService(HttpClient client, IOptionsSnapshot<ApplicationOptions> settings)
        {
            _client = client;
            _settings = settings.Value.ImagegalleryApiConfiguration;
        }

        public async Task PostImageGalleryApi(TokenResponse token, ImageForCreation image)
        {
            _client.SetBearerToken(token.AccessToken);

            var serializedImageForCreation = JsonConvert.SerializeObject(image);
            await _client.PostAsync(
                $"/api/images", new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"));
        }

        public Task PostImageGalleryApi(TokenResponse token, ImageForCreation image, bool waitForPostComplete, CancellationToken cancellation)
        {
            throw new System.NotImplementedException();
        }
    }
}
