using BuildingBlocks.Observability;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

/*add services to the container

Infrastructure - EF Core
Application - MediatR
API - Carter, HealthChecks, ...
*/

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration)
    .AddObservability("Ordering.API", builder.Configuration, opts => // Add OTEL(OpenTelemetry) 
    {
        opts.ConfigureMetrics = m =>
        {
            m.AddAspNetCoreInstrumentation(); // HTTP request metrics
            m.AddHttpClientInstrumentation(); // Outgoing HTTP calls metrics
            m.AddRuntimeInstrumentation();    // GC, memory, threads, CPU
        };
    
        opts.ConfigureTracer = t =>
        {
            t.AddAspNetCoreInstrumentation();
            t.AddHttpClientInstrumentation();
            t.AddSource("MassTransit"); // MassTransit 8 automatically create Activity
        };
    
        opts.ConfigureLogging = log => { };
    });

var app = builder.Build();

//configure the HTTP request pipeline
app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabaseAsync();
}

app.Run();
