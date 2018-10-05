using System;
using System.Globalization;
using System.Threading.Tasks;
using FlickrNet;
using ImageGallery.API.Client.Service.Classes;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.FlickrService;
using ImageGallery.FlickrService.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
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

        private readonly ILogger<FlickrSearchController> _logger;

        private readonly Trace _trace;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flickrSearchService"></param>
        /// <param name="flickrDownloadService"></param>
        public FlickrSearchController(IFlickrSearchService flickrSearchService,
            IFlickrDownloadService flickrDownloadService, ILogger<FlickrSearchController> logger)
        {
            _trace = Trace.Current.Child();
            _flickrSearchService = flickrSearchService ?? throw new ArgumentNullException(nameof(flickrSearchService));
            _flickrDownloadService = flickrDownloadService ?? throw new ArgumentNullException(nameof(flickrDownloadService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// FlickrSearch:PhotoInfo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _trace.Record(Annotations.ClientSend());
            _trace.Record(Annotations.ServiceName("FlickrSearch:PhotoInfo"));
            _trace.Record(Annotations.Rpc("GET"));

            var result = await _flickrSearchService.GetPhotoInfoAsync("7012518889");

            _trace.Record(Annotations.ClientRecv());

            return Ok(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> PostFlickrImage()
        {
            PhotoSearchOptions photoSearchOptions = new PhotoSearchOptions
            {
                // See for List of Available Machine Tags
                //https://api-attractions.navigatorglass.com/swagger/#!/MachineKey/ApiMachineKeyPredicatesGet

                MachineTags = "machine_tags => nycparks:",
                //MachineTags = "machine_tags => nychalloffame:",
                //MachineTags = "machine_tags => nycparks:m010=",
                //MachineTags = "machine_tags => nycparks:m089=",
                //MachineTags = "machine_tags => nycparks:q436=",
                // MachineTags = "machine_tags => nycparks:m010=114",
                // UserId = "",
                //PhotoSize = "q",   //150x150
                //PhotoSize = "z",   // Medium 640
                //PhotoSize = "q", //width="1024" height="768
            };

            Log.Information("Begin Zipkin Trace Here");
            _trace.Record(Annotations.ClientSend());
            _trace.Record(Annotations.ServiceName("FlickrSearch:PostFlickrImage"));
            _trace.Record(Annotations.Rpc("Post"));
            var photos = await _flickrSearchService.SearchPhotosAsync(photoSearchOptions);
            _trace.Record(Annotations.ClientRecv());
            _logger.LogInformation("Total Photos:{PhotoCount}", photos.Count);


            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-InlineCount");
            HttpContext.Response.Headers.Add("X-InlineCount", photos.Count.ToString(CultureInfo.InvariantCulture));
            return Ok(photos);
        }

    }
}
