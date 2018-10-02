using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.FlickrService.Helpers;
using Serilog;

namespace ImageGallery.API.Client.Service.Services
{
    public class FlickrDownloadService : IFlickrDownloadService
    {
        private readonly HttpClient _client;

        public FlickrDownloadService(HttpClient client)
        {
            _client = client;
        }

        public async Task<byte[]> GetFlickrImageAsync(string url)
        {
            var response = await _client.GetAsync(url);
            var results = await response.Content.ReadAsByteArrayAsync();

            return results;
        }

        public async Task<byte[]> GetFlickrImageAsync(string url, CancellationToken cancellation)
        {
            var response = await _client.GetAsync(url, cancellation);
            var results = await response.Content.ReadAsByteArrayAsync();

            Log.Information("{@Status} {@Url} Image Download", response.StatusCode, url);

            return results;
        }

        public async Task<byte[]> GetFlickrImageAsync(Photo photo, string size)
        {
            var response = await _client.GetAsync(photo.GetPhotoUrl(size));
            var results = await response.Content.ReadAsByteArrayAsync();

            return results;
        }

        public Task<byte[]> GetFlickrImageAsync(Photo photo, string size, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
