using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

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
                .ConfigureResource(r =>
                {
                    // set service name, version, instance id for all telemetry types (traces, metrics, logs)
                    r.AddService(
                        serviceName: serviceName,
                        serviceVersion: Assembly.GetEntryAssembly()?.GetName().Version?.ToString(),
                        serviceInstanceId: Environment.MachineName);

                    // add common attributes for all telemetry types
                    r.AddAttributes(new[]
                    {
                        new KeyValuePair<string, object>("deployment.environment", config["ASPNETCORE_ENVIRONMENT"] ?? "unknown"), // e.g. "Development", "Staging", "Production"
                        new KeyValuePair<string, object>("host.name", Environment.MachineName)
                    });
                })
                .WithMetrics(m =>
                {
                    // custom setting
                    options.ConfigureMetrics?.Invoke(m);

                    // add some common meters for ASP.NET Core applications
                    // for examle metrics about HTTP requests, Kestrel server, outgoing HTTP calls, GC, etc. will be automatically collected if these meters are added.
                    m.AddMeter(
                        "Microsoft.AspNetCore.Hosting",
                        "Microsoft.AspNetCore.Server.Kestrel",
                        "System.Net.Http",
                        "System.Runtime"
                    );

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

                    // Messaging
                    t.AddSource("MassTransit"); // MassTransit 8 automatically create Activity

                    // sample because 100% of traces may be too much for production, but it's good for development and testing.
                    var samplingRatio = config.GetValue<double?>("Observability:Tracing:SamplingRatio") ?? 1.0;
                    t.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(samplingRatio))); // sample all traces by default

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

            // if ReplaceLoggingProviders is true, it will clear all existing logging providers and only use OpenTelemetry logging provider to send logs to OTLP.
            if (config.GetValue<bool>("Observability:ReplaceLoggingProviders"))
            {
                builder.ClearProviders(); // replace default logging with OTL logging
            }

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
