using FlickrNet;

namespace ImageGallery.FlickrService.Helpers
{
    public static class PhotoHelpers
    {
        /// <summary>
        /// Get Download Link for photo by photo size
        /// </summary>
        /// <param name="photo"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetPhotoUrl(this Photo photo, string size)
        {
            string url = $"https://farm{photo.Farm}.staticflickr.com/{photo.Server}/{photo.PhotoId}_{photo.Secret}_{size}.jpg";
            return url;
        }
    }
}
