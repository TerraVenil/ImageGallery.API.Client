using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace ImageGallery.API.Client.Service.Services
{
    public class ImageGalleryCommandService : IImageGalleryCommandService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ImagegalleryApiConfiguration _settings;

        public ImageGalleryCommandService(IHttpClientFactory httpClientFactory, IOptionsSnapshot<ApplicationOptions> settings)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value.ImagegalleryApiConfiguration;
        }

        public async Task PostImageGalleryApi(TokenResponse token, ImageForCreation image, CancellationToken cancellation)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://imagegallery-api.informationcart.com");
            client.SetBearerToken(token.AccessToken);

            var serializedImageForCreation = JsonConvert.SerializeObject(image);

            await client.PostAsync(
                    $"/api/images",
                    new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"), cancellation)
                    .ContinueWith(r =>
                    {
                        if (!r.Result.IsSuccessStatusCode)
                        {
                            Log.Error("{@Status} ImageGalleryCommand Post Error {@Image}", r.Result.StatusCode.ToString(), image.ToString());
                        }
                        else
                        {
                            Log.Information("{@Status} ImageGalleryCommand Post Complete {@Image}", r.Result.StatusCode, image.ToString());
                        }
                    }, cancellation);
        }

        public Task PostImageGalleryApi(TokenResponse token, ImageForCreation image, bool waitForPostComplete, CancellationToken cancellation)
        {
            throw new System.NotImplementedException();
        }
    }
}
