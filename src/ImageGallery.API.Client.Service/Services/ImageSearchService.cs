﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet;
using ImageGallery.API.Client.Service.Classes;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using ImageGallery.FlickrService;
using ImageGallery.FlickrService.Helpers;
using Microsoft.Extensions.Logging;
using Serilog;


namespace ImageGallery.API.Client.Service.Services
{
    public class ImageSearchService : IImageSearchService
    {
        private readonly ISearchService _searchService;

        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public ImageSearchService(ISearchService searchService, ILogger<ImageSearchService> logger)
        {
            this._searchService = searchService;
            this._logger = logger;
        }

        public async Task<IEnumerable<ImageForCreation>> GetImagesAsync(int maxImagesCount = 0)
        {
            var photoSearchOptions = new PhotoSearchOptions()
            {
                MachineTags = "machine_tags => nychalloffame:",
                Extras = PhotoSearchExtras.All,
            };

            List<ImageForCreation> imageForCreations = new List<ImageForCreation>();

            var photos = await _searchService.SearchPhotosAsync(photoSearchOptions);

            var list = maxImagesCount == 0 ? photos : photos.Take(maxImagesCount);
            foreach (var photo in list)
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

        public bool IsSearchRunning
        {
            get => _isSearchRunning;
            private set => _isSearchRunning = value;
        }

        public ConcurrentQueue<ImageForCreation> ImageForCreations { get; } = new ConcurrentQueue<ImageForCreation>();

        private volatile int _asynCounter;
        private volatile bool _isSearchRunning;

        public void StartImagesSearchQueue(SearchOptions options, int maxThreads)
        {
            if (_isSearchRunning) return;
            _isSearchRunning = true;

            ThreadPool.QueueUserWorkItem(async s =>
            {
                try
                {
                    while (!ImageForCreations.IsEmpty)
                        ImageForCreations.TryDequeue(out _);

                    var photoSearchOptions = new PhotoSearchOptions
                    {
                        UserId = !string.IsNullOrEmpty(options.UserId) ? options.UserId : null,
                        MachineTags = !string.IsNullOrEmpty(options.MachineTags) ? options.MachineTags : null,
                        Extras = PhotoSearchExtras.All,
                    };

                    //start remote queue
                    await _searchService.StartPhotosSearchQueueAsync(photoSearchOptions).ConfigureAwait(false);

                    while (_searchService.IsSearchQueueRunning || _asynCounter > 0 || _searchService.PhotosQueue.Count > 0)
                    {
                        //fetch photos from remote queue
                        if (!_searchService.PhotosQueue.TryDequeue(out var photo))
                            continue;
                        //wait for available threads
                        while (_asynCounter > maxThreads)
                        {
                            await Task.Delay(5);
                        }

                        //use local queue to prepare image
                        _asynCounter++;
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            try
                            {
                                PrepareImage(photo, options.PhotoSize);
                            }
                            finally
                            {
                                _asynCounter--;
                            }
                        });
                    }

                    // Console.WriteLine($"StartImagesSearchQueue ends! {ImageForCreations.Count} left, async counter: {_asynCounter}");
                }
                finally
                {
                    _isSearchRunning = false;
                }
            });
        }

        private void PrepareImage(Photo photo, string size)
        {
            var image = new ImageForCreation
            {
                Title = photo.Title,
                Category = "Flickr Batch",
                PhotoId = long.Parse(photo.PhotoId),
                DataSource = "Flickr"
            };

            var photoUrl = photo.GetPhotoUrl(size);
            var request = (HttpWebRequest)WebRequest.Create(photoUrl);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var inputStream = response.GetResponseStream())
                {
                    Log.Information("{@Status} Flickr Get Image Complete {@Image}", response.StatusCode, image.ToString());
                    using (var ms = new MemoryStream())
                    {
                        inputStream.CopyTo(ms);
                        image.Bytes = ms.ToArray();
                    }
                }
                //put image into queue
                ImageForCreations.Enqueue(image);
            }
        }


    }
}
