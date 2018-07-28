using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public ConcurrentQueue<Photo> PhotosQueue { get; } = new ConcurrentQueue<Photo>();
        public bool IsSearchQueueRunning { get; private set; }

        public async Task StartPhotosSearchQueueAsync(PhotoSearchOptions photoSearchOptions)
        {
            if(IsSearchQueueRunning) return;
            try
            {
                IsSearchQueueRunning = true;
                while (!PhotosQueue.IsEmpty)
                    PhotosQueue.TryDequeue(out _);

                var defaultPageSize = photoSearchOptions.PerPage != 0 ? photoSearchOptions.Page : DefaultPageSize;
                var x = await _flickr.PhotosSearchAsync(photoSearchOptions);
                var total = x.Total;

                var count = PagingUtil.CalculateNumberOfPages(total, defaultPageSize);
                var pages = new List<int>();
                for (int i = 1; i <= count; i++)
                    pages.Add(i);

                await AsyncHelper.RunWithMaxDegreeOfConcurrency(30, pages, async page =>
                {
                    //copy options
                    var o = new PhotoSearchOptions();
                    foreach (var property in typeof(PhotoSearchOptions).GetProperties().Where(a=> a.CanWrite && a.SetMethod != null && a.SetMethod.IsPublic))
                        property.SetValue(o, property.GetValue(photoSearchOptions));

                    o.Page = page;
                    var photoCollection = await _flickr.PhotosSearchAsync(o);
                    foreach (var photo in photoCollection)
                    {
                        PhotosQueue.Enqueue(photo);
                    }
                });
            }
            finally
            {
                IsSearchQueueRunning = false;
            }
        }
    }
}
