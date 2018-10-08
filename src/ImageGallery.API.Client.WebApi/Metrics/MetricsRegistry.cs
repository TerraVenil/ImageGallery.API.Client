using App.Metrics;
using App.Metrics.Apdex;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Histogram;
using App.Metrics.Meter;
using App.Metrics.ReservoirSampling.Uniform;
using App.Metrics.Timer;

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

        public static ApdexOptions ApdexOne => new ApdexOptions
        {
            Name = "apdex_one",
            AllowWarmup = false,
            ApdexTSeconds = 0.1,
            Reservoir = () => new DefaultAlgorithmRReservoir()
        };

        public static CounterOptions CounterOne => new CounterOptions
        {
            Name = "counter_one"
        };

        public static CounterOptions CounterWithSetItems => new CounterOptions
        {
            Name = "counter_withitems",
            ReportSetItems = false
        };

        public static GaugeOptions GaugeOne => new GaugeOptions
        {
            Name = "gauge_one"
        };

        public static HistogramOptions HistogramOne => new HistogramOptions
        {
            Name = "histogram_one"
        };

        public static MeterOptions MeterOne => new MeterOptions
        {
            Name = "meter_one"
        };

        public static MeterOptions MeterWithSetItems => new MeterOptions
        {
            Name = "meter_withitems",
            ReportSetItems = false
        };

        public static TimerOptions TimerOne => new TimerOptions
        {
            Name = "timer_one"
        };
    }
}



