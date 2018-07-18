using System.Collections.Generic;
using System.Threading.Tasks;
using FlickrNet;

namespace ImageGallery.FlickrService
{
    public interface ISearchService
    {
        Task<PhotoInfo> GetPhotoInfoAsync(string photoId);

        Task<IList<Photo>> SearchPhotosAsync(PhotoSearchOptions photoSearchOptions);
    }
}
