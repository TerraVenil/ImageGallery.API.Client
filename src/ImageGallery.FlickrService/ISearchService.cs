using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet;

namespace ImageGallery.FlickrService
{
    public interface ISearchService
    {
        Task<PhotoInfo> GetPhotoInfoAsync(string photoId);

        Task<IList<Photo>> SearchPhotosAsync(PhotoSearchOptions photoSearchOptions);
        Task StartPhotosSearchQueueAsync(CancellationToken searchOptions, PhotoSearchOptions photoSearchOptions);
        ConcurrentQueue<Photo> PhotosQueue { get; }
        bool IsSearchQueueRunning { get; }
        int FlickrQueriesCount { get; }
    }
}
