using App.Metrics;
using App.Metrics.Counter;

namespace ImageGallery.API.Client.WebApi.Metrics
{
    /// <summary>
    ///
    /// </summary>
    public static class MetricsRegistry
    {
        /// <summary>
        ///
        /// </summary>
        public static CounterOptions SampleCounter => new CounterOptions
        {
            Name = "Sample Counter",
            MeasurementUnit = Unit.Calls
        };
    }
}
