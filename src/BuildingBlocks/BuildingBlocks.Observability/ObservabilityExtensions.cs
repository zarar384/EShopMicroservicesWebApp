using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Observability
{
    public static class ObservabilityExtensions
    {
        public static IServiceCollection AddObservability(
            this IServiceCollection services,
            string serviceName,
            IConfiguration config,
            Action<ObservabilityOptions>? configureOptions = null)
        {
            var endpoint = config.GetValue<string>("Otlp:Endpoint");

            if (endpoint == null) return services;

            var options = new ObservabilityOptions();
            configureOptions?.Invoke(options);

            services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName))
                .WithMetrics(m =>
                {
                    // custom setting
                    options.ConfigureMetrics?.Invoke(m);

                    m.AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(endpoint);
                        o.Protocol = OtlpExportProtocol.Grpc;
                    });
                })
                .WithTracing(t =>
                {
                    // custom setting
                    options.ConfigureTracer?.Invoke(t);

                    t.AddOtlpExporter(o =>  // send to OTLP
                    {
                        o.Endpoint = new Uri(endpoint);
                        o.Protocol = OtlpExportProtocol.Grpc;
                    });
                });

            services.AddLogging(builder =>
            {
                builder.AddObservabilityLogging(serviceName, config, options.ConfigureLogging);
            });

            return services;
        }

        public static void AddObservabilityLogging(
            this ILoggingBuilder builder,
            string serviceName,
            IConfiguration config,
            Action<OpenTelemetryLoggerOptions>? configure = null)
        {
            var endpoint = config.GetValue<string>("Otlp:Endpoint");

            if (endpoint == null) return;

            builder.ClearProviders(); // replace default logging with OTL logging
            builder.AddOpenTelemetry(o =>
            {
                o.IncludeFormattedMessage = true; // render log message
                o.IncludeScopes = true;
                o.ParseStateValues = true;

                // custom setting
                configure?.Invoke(o);

                o.AddOtlpExporter(opt =>  // send to OTLP
                {
                    opt.Endpoint = new Uri(endpoint);
                    opt.Protocol = OtlpExportProtocol.Grpc;
                });
            });
        }
    }
}
