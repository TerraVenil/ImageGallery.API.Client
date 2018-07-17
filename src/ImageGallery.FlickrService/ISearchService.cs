using System.Collections.Generic;
using FlickrNet;

namespace ImageGallery.FlickrService
{
    public interface ISearchService
    {
        PhotoInfo GetPhotoInfo(string photoId);

        IList<PhotoInfo> GetPhotoInfoList(IEnumerable<string> photoIdList);

        IList<Photo> SearchPhotos(PhotoSearchOptions photoSearchOptions);
    }
}
