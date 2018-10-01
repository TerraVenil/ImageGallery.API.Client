using System;
using System.Threading.Tasks;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ImageGallery.API.Client.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    public class ImageGalleryQueryController : ControllerBase
    {
        private readonly IImageGalleryQueryService _imageGalleryQueryService;

        private readonly ITokenProvider _tokenProvider;

        private readonly ImagegalleryApiConfiguration _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageGalleryQueryService"></param>
        /// <param name="tokenProvider"></param>
        /// <param name="settings"></param>
        public ImageGalleryQueryController(IImageGalleryQueryService imageGalleryQueryService,
            ITokenProvider tokenProvider, IOptionsSnapshot<ApplicationOptions> settings)
        {
            this._tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(imageGalleryQueryService));
            this._imageGalleryQueryService = imageGalleryQueryService ?? throw new ArgumentNullException(nameof(tokenProvider));
            _settings = settings.Value.ImagegalleryApiConfiguration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var token = await _tokenProvider.RequestResourceOwnerPasswordAsync(_settings.Login, _settings.Password, _settings.Api);
            var result = await _imageGalleryQueryService.GetUserImageCollectionAsync(token);

            return Ok(result);
        }
    }
}
