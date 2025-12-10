using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Observability
{
    public class ObservabilityOptions
    {
        public Action<MeterProviderBuilder>? ConfigureMetrics { get; set;  }
        public Action<TracerProviderBuilder>? ConfigureTracer { get; set; }
        public Action<OpenTelemetryLoggerOptions?> ConfigureLogging { get; set; }
    }
}
