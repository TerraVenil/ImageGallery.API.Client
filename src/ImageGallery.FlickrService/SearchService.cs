using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet;
using ImageGallery.FlickrService.Helpers;
using Polly;
using Serilog;

namespace ImageGallery.FlickrService
{
    public class SearchService : ISearchService
    {
        private readonly Flickr _flickr;

        private const int DefaultPageSize = 50;

        protected volatile int _flickrQueriesCount;
        public int FlickrQueriesCount => _flickrQueriesCount;

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
            _flickrQueriesCount = 0;
            var photos = new List<Photo>();
            var defaultPageSize = photoSearchOptions.PerPage != 0 ? photoSearchOptions.Page : DefaultPageSize;
            var total = _flickr.PhotosSearchAsync(photoSearchOptions).Result.Total;
            _flickrQueriesCount++;

            var pages = PagingUtil.CalculateNumberOfPages(total, defaultPageSize);

            for (int i = 1; i <= pages; i++)
            {
                photoSearchOptions.Page = i;
                var photoCollection = await _flickr.PhotosSearchAsync(photoSearchOptions);
                _flickrQueriesCount++;
                photos.AddRange(photoCollection);
            }

            return photos;
        }

        public ConcurrentQueue<Photo> PhotosQueue { get; } = new ConcurrentQueue<Photo>();

        public bool IsSearchQueueRunning { get; private set; }
        public static int ThreadsCount;

        public async Task StartPhotosSearchQueueAsync(CancellationToken cancellation, PhotoSearchOptions photoSearchOptions)
        {
            if (IsSearchQueueRunning) return;
            ThreadsCount = 0;
            _flickrQueriesCount = 0;
            try
            {
                IsSearchQueueRunning = true;
                while (!PhotosQueue.IsEmpty)
                {
                    if (cancellation.IsCancellationRequested)
                        return;
                    PhotosQueue.TryDequeue(out _);
                }

                var policy = Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(
                        retryCount: RetriesCount,
                        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200), // Wait 200ms between each try.
                        onRetry: (exception, calculatedWaitDuration) => // Capture some info for logging!
                        {
                            Log.Error("{@Status} ImageGalleryAPI Web Query Error! Retrying after {ex}", "ERROR", exception.InnerException?.Message ?? exception.Message);
                        });

                var defaultPageSize = photoSearchOptions.PerPage != 0 ? photoSearchOptions.Page : DefaultPageSize;
                var x = await policy.ExecuteAsync(async()=> await _flickr.PhotosSearchAsync(photoSearchOptions));
                _flickrQueriesCount++;
                var total = x.Total;

                var count = PagingUtil.CalculateNumberOfPages(total, defaultPageSize);
                var pages = new List<int>();
                for (int i = 1; i <= count; i++)
                    pages.Add(i);

                await AsyncHelper.RunWithMaxDegreeOfConcurrency(cancellation, 30, pages, async page =>
                {
                    if (cancellation.IsCancellationRequested)
                        return;
                    ThreadsCount++;
                    try
                    {
                        //copy options
                        var o = new PhotoSearchOptions();
                        foreach (var property in typeof(PhotoSearchOptions).GetProperties().Where(a => a.CanWrite && a.SetMethod != null && a.SetMethod.IsPublic))
                            property.SetValue(o, property.GetValue(photoSearchOptions));

                        o.Page = page;
                        var photoCollection = await policy.ExecuteAsync(async token => await _flickr.PhotosSearchAsync(o), cancellation);
                        _flickrQueriesCount++;
                        foreach (var photo in photoCollection)
                        {
                            if (cancellation.IsCancellationRequested)
                                return;
                            PhotosQueue.Enqueue(photo);
                            // photo.PrintPhoto();
                            Log.Information("{@Page} Flickr Enqueue Image Metadata Complete {@PhotoId}|{@Title}|{@LastUpdated}", photoCollection.Page, photo.PhotoId, photo.Title,
                                photo.LastUpdated);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO logging
                    }
                    finally
                    {
                        ThreadsCount--;
                        Debug.WriteLine($"SS: {ThreadsCount}");
                    }
                });
            }
            finally
            {
                IsSearchQueueRunning = false;
            }
        }

        public int RetriesCount { get; set; } = 3;
    }
}
