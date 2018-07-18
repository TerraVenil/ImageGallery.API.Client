﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FlickrNet;
using ImageGallery.FlickrService.Helpers;

namespace ImageGallery.FlickrService
{
    public class SearchService : ISearchService
    {
        private readonly Flickr _flickr;

        private const int DefaultPageSize = 50;
        public SearchService(string apiKey, string secret)
        {
            this._flickr = new Flickr(apiKey, secret);
        }

        public async Task<PhotoInfo> GetPhotoInfoAsync(string photoId)
        {
            var photoInfo = await _flickr.PhotosGetInfoAsync(photoId);  
            return photoInfo;
        }

        public async Task<IList<Photo>> SearchPhotosAsync(PhotoSearchOptions photoSearchOptions)
        {
            var photos = new List<Photo>();
            var defaultPageSize = photoSearchOptions.PerPage != 0 ? photoSearchOptions.Page : DefaultPageSize;
            var total = _flickr.PhotosSearchAsync(photoSearchOptions).Result.Total;

            var pages = PagingUtil.CalculateNumberOfPages(total, defaultPageSize);

            for (int i = 1; i <= pages; i++)
            {
                photoSearchOptions.Page = i;
                var photoCollection = await _flickr.PhotosSearchAsync(photoSearchOptions);
                photos.AddRange(photoCollection);
            };

            return photos;
        }

    }
}
