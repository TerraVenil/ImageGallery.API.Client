using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlickrNet;

namespace ImageGallery.FlickrService
{
    public interface ISearchService
    {
        Task<PhotoInfo> GetPhotoInfoAsync(string photoId);

        Task<IList<Photo>> SearchPhotosAsync(PhotoSearchOptions photoSearchOptions);
        Task StartPhotosSearchQueueAsync(PhotoSearchOptions photoSearchOptions);
        ConcurrentQueue<Photo> PhotosQueue { get; }
        bool IsSearchQueueRunning { get; }
    }
}
