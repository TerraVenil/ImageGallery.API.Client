using System;
using System.Threading.Tasks;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.FlickrService;
using ImageGallery.FlickrService.Helpers;
using Microsoft.AspNetCore.Mvc;
using zipkin4net;

namespace ImageGallery.API.Client.WebApi.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FlickrSearchController : ControllerBase
    {
        private readonly IFlickrSearchService _flickrSearchService;

        private readonly IFlickrDownloadService _flickrDownloadService;

        private readonly Trace _trace;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flickrSearchService"></param>
        /// <param name="flickrDownloadService"></param>
        public FlickrSearchController(IFlickrSearchService flickrSearchService, IFlickrDownloadService flickrDownloadService)
        {
            _trace = Trace.Create();
            _flickrSearchService = flickrSearchService ?? throw new ArgumentNullException(nameof(flickrSearchService));
            _flickrDownloadService = flickrDownloadService ?? throw new ArgumentNullException(nameof(flickrDownloadService));
        }


        /// <summary>
        /// FlickrSearch:PhotoInfo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _trace.Record(Annotations.ServiceName("FlickrSearch:PhotoInfo"));
            _trace.Record(Annotations.ServerRecv());

            var result = await _flickrSearchService.GetPhotoInfoAsync("7012518889");
            _trace.Record(Annotations.ServerSend());

            return Ok(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> PostFlickrImage(string id)
        {
            var size = "n";
            var photoInfo = await _flickrSearchService.GetPhotoInfoAsync(id);
            var url = photoInfo.GetPhotoUrl(size);

            var result = await _flickrDownloadService.GetFlickrImageAsync(url);
            return Ok();
        }

    }
}
