using System.Threading;
using System.Threading.Tasks;
using FlickrNet;

namespace ImageGallery.API.Client.Service.Interface
{
    public interface IFlickrDownloadService
    {
        Task<byte[]> GetFlickrImageAsync(string url);

        Task<byte[]> GetFlickrImageAsync(Photo photo, string size);

        Task<byte[]> GetFlickrImageAsync(Photo photo, string size, CancellationToken cancellation);
    }

}
