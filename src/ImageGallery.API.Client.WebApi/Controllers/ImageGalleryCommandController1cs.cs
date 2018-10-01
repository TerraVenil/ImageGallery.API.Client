using System;
using System.Threading.Tasks;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using ImageGallery.FlickrService;
using ImageGallery.FlickrService.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ImageGallery.API.Client.WebApi.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Route("api/[controller]")]
    public class ImageGalleryCommandController1cs : ControllerBase
    {
        private readonly IImageGalleryCommandService _imageGalleryCommandService;

        private readonly IFlickrSearchService _flickrSearchService;

        private readonly IFlickrDownloadService _flickrDownloadService;

        private readonly ITokenProvider _tokenProvider;

        private readonly ImagegalleryApiConfiguration _settings;

        ///  <summary>
        /// 
        ///  </summary>
        ///  <param name="imageGalleryCommandService"></param>
        /// <param name="flickrDownloadService"></param>
        /// <param name="flickrSearchService"></param>
        ///  <param name="tokenProvider"></param>
        ///  <param name="settings"></param>
        public ImageGalleryCommandController1cs(IImageGalleryCommandService imageGalleryCommandService, IFlickrDownloadService flickrDownloadService, IFlickrSearchService flickrSearchService,
            ITokenProvider tokenProvider, IOptionsSnapshot<ApplicationOptions> settings)
        {
            this._tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            this._flickrSearchService = flickrSearchService ?? throw new ArgumentNullException(nameof(flickrSearchService));
            this._flickrDownloadService = flickrDownloadService ?? throw new ArgumentNullException(nameof(flickrDownloadService));
            this._imageGalleryCommandService = imageGalleryCommandService ?? throw new ArgumentNullException(nameof(imageGalleryCommandService));
            _settings = settings.Value.ImagegalleryApiConfiguration;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoId"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string photoId, string size)
        {
            var token = await _tokenProvider.RequestResourceOwnerPasswordAsync(_settings.Login, _settings.Password, _settings.Api);
            var photoInfo = await _flickrSearchService.GetPhotoInfoAsync(photoId);
            var photo = await _flickrDownloadService.GetFlickrImageAsync(photoInfo.GetPhotoUrl(size));

            var image = new ImageForCreation
            {
                Title = photoInfo.Title,
                Bytes = photo,
                PhotoId = long.Parse(photoInfo.PhotoId),
                Category = "API Post",
                DataSource = "Flickr"
            };

            await _imageGalleryCommandService.PostImageGalleryApi(token, image);

            return Ok();
        }

    }
}
