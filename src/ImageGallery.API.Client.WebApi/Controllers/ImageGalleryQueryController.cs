using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Helpers;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ImageGallery.API.Client.WebApi.Controllers
{
    /// <summary>
    /// Image Gallery Query Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
            this._tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            this._imageGalleryQueryService = imageGalleryQueryService ?? throw new ArgumentNullException(nameof(imageGalleryQueryService));
            _settings = settings.Value.ImagegalleryApiConfiguration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ImageModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            Console.WriteLine($"{_settings.Login}_{_settings.Password}_{_settings.Api}");

            var token = await _tokenProvider.RequestResourceOwnerPasswordAsync(_settings.Login, _settings.Password, _settings.Api);
            token.Show();

            var result = await _imageGalleryQueryService.GetUserImageCollectionAsync(token);

            return Ok(result);
        }
    }
}
