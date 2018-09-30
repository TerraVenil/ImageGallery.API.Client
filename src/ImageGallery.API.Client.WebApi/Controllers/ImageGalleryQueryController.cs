using System.Threading.Tasks;
using ImageGallery.API.Client.Service.Interface;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageGalleryQueryService"></param>
        public ImageGalleryQueryController(IImageGalleryQueryService imageGalleryQueryService, ITokenProvider tokenProvider)
        {
            this._tokenProvider = tokenProvider;
            this._imageGalleryQueryService = imageGalleryQueryService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var token = await _tokenProvider.RequestResourceOwnerPasswordAsync("Mary", "password", "imagegalleryapi");
            var result = await _imageGalleryQueryService.GetUserImageCollectionAsync(token);

            return Ok(result);
        }
    }
}
