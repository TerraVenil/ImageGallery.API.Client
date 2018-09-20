using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet;
using ImageGallery.API.Client.Service.Classes;
using ImageGallery.API.Client.Service.Models;

namespace ImageGallery.API.Client.Service.Interface
{
    public interface IImageSearchService
    {
        Task<IEnumerable<ImageForCreation>>  GetImagesAsync(int maxImagesCount = 0);

        void StartImagesSearchQueue(CancellationToken cancellation, SearchOptions options, int maxThreads, HttpClient httpClient);

        ConcurrentQueue<ImageForCreation> ImageForCreations { get; }

        bool IsSearchRunning { get; }

        int FlickrQueriesCount { get; }

        ulong FlickrQueriesBytes { get; }

    }
}
