using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageGallery.API.Client.Service.Services
{
    public class ImageGalleryQueryService : IImageGalleryQueryService
    {
        private readonly HttpClient _client;

        public ImageGalleryQueryService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<ImageModel>> GetUserImageCollectionAsync(TokenResponse token)
        {
            List<ImageModel> imageList = new List<ImageModel>();

            _client.SetBearerToken(token.AccessToken);

            var response = await _client.GetAsync($"/api/images");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                imageList = JsonConvert.DeserializeObject<List<ImageModel>>(content);
                Console.WriteLine(JArray.Parse(content));
                Console.WriteLine($"ImagesCount:{imageList.Count}");
                return imageList;
            }

            return imageList;
        }

        public Task<List<ImageModel>> GetUserImageCollectionAsync(TokenResponse token, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
