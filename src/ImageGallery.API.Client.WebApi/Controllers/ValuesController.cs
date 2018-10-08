using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using App.Metrics;
using ImageGallery.API.Client.WebApi.Metrics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using App.Metrics.Apdex;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Histogram;
using App.Metrics.Meter;
using App.Metrics.ReservoirSampling.Uniform;
using App.Metrics.Timer;


namespace ImageGallery.API.Client.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMetrics _metrics;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<ValuesController> _logger;

        private static readonly Random Rnd = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="metrics"></param>
        public ValuesController(IHttpClientFactory httpClientFactory, IMetrics metrics, ILogger<ValuesController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _metrics = metrics;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var client = _httpClientFactory.CreateClient();
            var result = await client.GetStringAsync("http://www.google.com");

            foreach (var unused in Enumerable.Range(0, 10))
            {
                // _metrics.Measure.Apdex.Track(MetricsRegistry.ApdexOne, () => Metrics.Clock.Advance(TimeUnit.Milliseconds, Rnd.Next(5000)));
                _metrics.Measure.Counter.Increment(MetricsRegistry.CounterOne);
                _metrics.Measure.Counter.Increment(MetricsRegistry.CounterWithSetItems, "item1");
                _metrics.Measure.Gauge.SetValue(MetricsRegistry.GaugeOne, Rnd.Next(0, 100));
                _metrics.Measure.Histogram.Update(MetricsRegistry.HistogramOne, Rnd.Next(0, 100));
                _metrics.Measure.Meter.Mark(MetricsRegistry.MeterOne, Rnd.Next(0, 100));
                _metrics.Measure.Meter.Mark(MetricsRegistry.MeterWithSetItems, Rnd.Next(0, 100), "item1");
            }



            _metrics.Measure.Counter.Increment(MetricsRegistry.SampleCounter);

            return Ok(result);
        }


        /// <summary>
        /// GET api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// POST api/values
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// PUT api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// DELETE api/values/5
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
