using App.Metrics;
using App.Metrics.Counter;

namespace ImageGallery.API.Client.WebApi.Metrics
{
    public static class MetricsRegistry
    {
        public static CounterOptions SampleCounter => new CounterOptions
        {
            Name = "Sample Counter",
            MeasurementUnit = Unit.Calls
        };
    }
}
