using System.Net.Http;
using System.Threading.Tasks;
using App.Metrics;
using ImageGallery.API.Client.WebApi.Metrics;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="metrics"></param>
        public ValuesController(IHttpClientFactory httpClientFactory, IMetrics metrics)
        {
            _httpClientFactory = httpClientFactory;
            _metrics = metrics;
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
