using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using ImageGallery.FlickrService;
using ImageGallery.FlickrService.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImageGallery.API.Client.WebApi.Controllers
{
    /// <summary>
    /// ImageGallery Command Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImageGalleryCommandController : ControllerBase
    {
        private readonly IImageGalleryCommandService _imageGalleryCommandService;

        private readonly IFlickrSearchService _flickrSearchService;

        private readonly IFlickrDownloadService _flickrDownloadService;

        private readonly ILogger<ImageGalleryCommandController> _logger;

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
        //public ImageGalleryCommandController(IImageGalleryCommandService imageGalleryCommandService, IFlickrDownloadService flickrDownloadService, IFlickrSearchService flickrSearchService,
        //    ITokenProvider tokenProvider, IOptionsSnapshot<ApplicationOptions> settings)
        //{
        //    this._tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        //    this._flickrSearchService = flickrSearchService ?? throw new ArgumentNullException(nameof(flickrSearchService));
        //    this._flickrDownloadService = flickrDownloadService ?? throw new ArgumentNullException(nameof(flickrDownloadService));
        //    this._imageGalleryCommandService = imageGalleryCommandService ?? throw new ArgumentNullException(nameof(imageGalleryCommandService));
        //    _settings = settings.Value.ImagegalleryApiConfiguration;
        //}

        public ImageGalleryCommandController(IFlickrDownloadService flickrDownloadService, IFlickrSearchService flickrSearchService,
            ITokenProvider tokenProvider, IOptionsSnapshot<ApplicationOptions> settings, ILogger<ImageGalleryCommandController> logger)
        {
            this._flickrSearchService = flickrSearchService ?? throw new ArgumentNullException(nameof(flickrSearchService));
            this._flickrDownloadService = flickrDownloadService ?? throw new ArgumentNullException(nameof(flickrDownloadService));
            //this._imageGalleryCommandService = imageGalleryCommandService ?? throw new ArgumentNullException(nameof(imageGalleryCommandService));
            this._tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _settings = settings.Value.ImagegalleryApiConfiguration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="photoId"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Post([FromBody]string id)  //[FromBody]string photoId, string size
        {
            var photoId = "9250911801";
            var size = "n";

            var token = await _tokenProvider.RequestResourceOwnerPasswordAsync(_settings.Login, _settings.Password, _settings.Api);
            var photoInfo = await _flickrSearchService.GetPhotoInfoAsync(photoId);
            if (photoInfo == null)
                return NotFound();

            var photo = await _flickrDownloadService.GetFlickrImageAsync(photoInfo.GetPhotoUrl(size));
            var image = new ImageForCreation
            {
                Title = photoInfo.Title,
                Bytes = photo,
                PhotoId = long.Parse(photoInfo.PhotoId),
                Category = "API Post",
                DataSource = "Flickr"
            };

            //var cancellationToken = new CancellationToken();
            //await _imageGalleryCommandService.PostImageGalleryApi(token, image, cancellationToken);

            return Ok(photoInfo);
        }

    }
}
