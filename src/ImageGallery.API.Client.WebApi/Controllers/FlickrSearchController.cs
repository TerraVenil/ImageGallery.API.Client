using System;
using System.Threading.Tasks;
using ImageGallery.FlickrService;
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

        private readonly Trace _trace;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flickrSearchService"></param>
        public FlickrSearchController(IFlickrSearchService flickrSearchService)
        {
            _trace = Trace.Create();
            _flickrSearchService = flickrSearchService ?? throw new ArgumentNullException(nameof(flickrSearchService));
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
    }
}
