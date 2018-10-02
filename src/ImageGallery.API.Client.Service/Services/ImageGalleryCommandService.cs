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
        private readonly HttpClient _httpClient;

        private readonly ImagegalleryApiConfiguration _settings;

        public ImageGalleryCommandService(HttpClient client, IOptionsSnapshot<ApplicationOptions> settings)
        {
            _httpClient = client;
            _settings = settings.Value.ImagegalleryApiConfiguration;
        }

        public async Task PostImageGalleryApi(TokenResponse token, ImageForCreation image, CancellationToken cancellation)
        {
            _httpClient.SetBearerToken(token.AccessToken);

            var serializedImageForCreation = JsonConvert.SerializeObject(image);

            await _httpClient.PostAsync(
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
    }
}
