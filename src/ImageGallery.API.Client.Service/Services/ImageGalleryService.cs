﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet;
using ImageGallery.API.Client.Service.Classes;
using ImageGallery.API.Client.Service.Helpers;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using ImageGallery.FlickrService;
using ImageGallery.FlickrService.Helpers;
using Microsoft.Extensions.Logging;
using Polly;
using Serilog;

namespace ImageGallery.API.Client.Service.Services
{
    public class ImageGalleryService : IImageGalleryService
    {
        private readonly IFlickrSearchService _flickrSearchService;

        private readonly object locker = new object();

        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        private HttpClient _client;

        public ImageGalleryService(IFlickrSearchService flickrSearchService, ILogger<ImageGalleryService> logger)
        {
            this._flickrSearchService = flickrSearchService;
            this._logger = logger;
        }

        protected ulong InternalFlickrQueriesBytes;

        protected int InternalFlickrQueriesCount => _flickrSearchService?.FlickrQueriesCount ?? 0;

        protected volatile int LocalFlickrQueriesCount;

        private volatile int _asynCounter;

        private volatile bool _isSearchRunning;

        public void StartImagesSearchQueue(CancellationToken cancellation, SearchOptions options, int maxThreads, HttpClient client)
        {
            if (_isSearchRunning) return;

            _client = client;
            if (_client == null)
            {
                _client = new HttpClient();
                _client.DefaultRequestHeaders.ConnectionClose = true;
            }

            _isSearchRunning = true;
            LocalFlickrQueriesCount = 0;
            InternalFlickrQueriesBytes = 0;
            _asynCounter = 0;
            ThreadPool.QueueUserWorkItem(async s =>
            {
                try
                {
                    while (!ImageForCreations.IsEmpty)
                    {
                        if (cancellation.IsCancellationRequested)
                            return;
                        ImageForCreations.TryDequeue(out _);
                    }

                    var photoSearchOptions = new PhotoSearchOptions
                    {
                        UserId = !string.IsNullOrEmpty(options.UserId) ? options.UserId : null,
                        MachineTags = !string.IsNullOrEmpty(options.MachineTags) ? options.MachineTags : null,
                        Extras = PhotoSearchExtras.All,
                    };

                    //start remote queue
                    await _flickrSearchService.StartPhotosSearchQueueAsync(photoSearchOptions, cancellation).ConfigureAwait(false);

                    var cfg = ConfigurationHelper.GetGeneralConfig();

                    var policy = Policy
                        .Handle<HttpRequestException>()
                        .WaitAndRetryAsync(
                            retryCount: cfg.QueryRetriesCount,
                            sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(cfg.QueryWaitBetweenQueries), // Wait 200ms between each try.
                            onRetry: (exception, calculatedWaitDuration) => // Capture some info for logging!
                            {
                                Log.Error("{@Status} ImageGalleryAPI Web Query Error! Retrying after {ex}", "ERROR", exception.InnerException?.Message ?? exception.Message);
                            });

                    while (_flickrSearchService.IsSearchQueueRunning || _asynCounter > 0 || _flickrSearchService.PhotosQueue.Count > 0)
                    {
                        if (cancellation.IsCancellationRequested)
                            return;
                        //fetch photos from remote queue
                        if (!_flickrSearchService.PhotosQueue.TryDequeue(out var photo))
                            continue;
                        //wait for available threads
                        while (_asynCounter > maxThreads)
                        {
                            await Task.Delay(5, cancellation);
                        }
                        _asynCounter++;

                        //use local queue to prepare image
                        new Thread(async state =>
                        {
                            if (cancellation.IsCancellationRequested)
                                return;
                            try
                            {
                                await PrepareImage(policy, photo, options.PhotoSize, cancellation);
                            }
                            finally
                            {
                                _asynCounter--;
                                Debug.WriteLine($"ISS: {_asynCounter}");
                            }
                        }).Start();
                    }

                    // Console.WriteLine($"StartImagesSearchQueue ends! {ImageForCreations.Count} left, async counter: {_asynCounter}");
                }
                finally
                {
                    _isSearchRunning = false;

                }
            });
        }

        public ConcurrentQueue<ImageForCreation> ImageForCreations { get; } = new ConcurrentQueue<ImageForCreation>();

        public bool IsSearchRunning
        {
            get => _isSearchRunning;
            private set => _isSearchRunning = value;
        }

        public int FlickrQueriesCount => InternalFlickrQueriesCount + LocalFlickrQueriesCount;

        public ulong FlickrQueriesBytes => InternalFlickrQueriesBytes;

        private void UpdateFlickrBytes(int value)
        {
            lock (locker)
            {
                InternalFlickrQueriesBytes += (ulong)value;
            }
        }

        private async Task PrepareImage(IAsyncPolicy policy, Photo photo, string size, CancellationToken cancellation)
        {
            var image = new ImageForCreation
            {
                Title = photo.Title,
                Category = "Flickr Batch",
                PhotoId = long.Parse(photo.PhotoId),
                DataSource = "Flickr"
            };

            LocalFlickrQueriesCount++;
            try
            {
                await policy.ExecuteAsync(async token =>
                {
                    using (var response = await _client.GetAsync(photo.GetPhotoUrl(size), token))
                    {
                        image.Bytes = await response.Content.ReadAsByteArrayAsync();
                        Log.Information("{@Status}  {@Image}", response.StatusCode, image.ToString());
                    }
                }, cancellation);
                UpdateFlickrBytes(image.Bytes.Length);
                //put image into queue
                ImageForCreations.Enqueue(image);
            }
            catch (Exception ex)
            {
                //TODO handle error
                Log.Error("{@Status} Flickr Get Image", "ERROR", ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
