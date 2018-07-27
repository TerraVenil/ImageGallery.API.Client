using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FlickrNet;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using ImageGallery.FlickrService;

namespace ImageGallery.API.Client.Service.Services
{
    public class ImageSearchService : IImageSearchService
    {
        private readonly ISearchService _searchService;

        public ImageSearchService(ISearchService searchService)
        {
            this._searchService = searchService;
        }

        public async Task<IEnumerable<ImageForCreation>> GetImagesAsync()
        {
            var photoSearchOptions = new PhotoSearchOptions()
            {
                MachineTags = "machine_tags => nychalloffame:",
                Extras = PhotoSearchExtras.All,
            };

            List<ImageForCreation> imageForCreations = new List<ImageForCreation>();

            var photos = await _searchService.SearchPhotosAsync(photoSearchOptions);

            foreach (var photo in photos.Take(10))
            {
                var image = new ImageForCreation
                {
                    Title = photo.Title,
                    Category = "Test",

                };

               var photoUrl = photo.Medium640Url;
               HttpWebRequest request = (HttpWebRequest)WebRequest.Create(photoUrl);
               HttpWebResponse response = (HttpWebResponse)request.GetResponse();
               using (Stream inputStream = response.GetResponseStream())
               {
                   using (var ms = new MemoryStream())
                   {
                       inputStream.CopyTo(ms);
                       image.Bytes = ms.ToArray();
                   }
                }

                imageForCreations.Add(image);
            }

            return imageForCreations;
        }
    }
}
