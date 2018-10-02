using System.Collections.Concurrent;
using System.Threading;
using ImageGallery.API.Client.Service.Classes;
using ImageGallery.API.Client.Service.Models;

namespace ImageGallery.API.Client.Service.Interface
{
    public interface IImageGalleryService
    {
        void StartImagesSearchQueue(int maxThreads, SearchOptions options, CancellationToken cancellation);

        ConcurrentQueue<ImageForCreation> ImageForCreations { get; }

        bool IsSearchRunning { get; }

        int FlickrQueriesCount { get; }

        ulong FlickrQueriesBytes { get; }
    }
}
