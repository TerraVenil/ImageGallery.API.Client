using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            List<ImageForCreation> imageForCreations = new List<ImageForCreation>();
            var photos = await _searchService.SearchPhotosAsync(null);



            return imageForCreations;
        }
    }
}
