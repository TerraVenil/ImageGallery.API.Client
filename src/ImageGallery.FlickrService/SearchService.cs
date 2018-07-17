using System;
using System.Collections.Generic;
using System.Text;
using FlickrNet;

namespace ImageGallery.FlickrService
{
    public class SearchService : ISearchService
    {
        private readonly Flickr flickr;

        public SearchService(string apiKey, string secret)
        {
            this.flickr = new Flickr(apiKey, secret);
        }

        public PhotoInfo GetPhotoInfo(string photoId)
        {
            throw new NotImplementedException();
            //var photoInfo = flickr.PhotosGetInfo(photoId);
            //return photoInfo;
        }

        public IList<PhotoInfo> GetPhotoInfoList(IEnumerable<string> photoIdList)
        {
            throw new NotImplementedException();
        }

        public IList<Photo> SearchPhotos(PhotoSearchOptions photoSearchOptions)
        {
            var photos = new List<Photo>();
            var total = flickr.PhotosSearchAsync(photoSearchOptions).Result.Total;

            //var searchOptions = photoSearchOptions;
            //var pages = PagingHelper.CalculatePages(total, photoSearchOptions.PerPage);

            //for (int i = 0; i <= pages; i++)
            //{
            //    searchOptions.Page = i;
            //    PhotoCollection photoCollection = flickr.PhotosSearchAsync(photoSearchOptions);
            //    photos.AddRange(photoCollection);
            //}

            return photos;
        }

    }
}
