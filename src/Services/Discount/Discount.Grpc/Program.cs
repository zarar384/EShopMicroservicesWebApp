using BuildingBlocks.Observability;
using Discount.Grpc.Data;
using Discount.Grpc.Services;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<DiscountContext>(opt => 
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add OTEL(OpenTelemetry) 
builder.Services.AddObservability("Discount.GRPC", builder.Configuration, opts =>
{
    opts.ConfigureMetrics = m =>
    {
        m.AddAspNetCoreInstrumentation(); // also gRPC server metrics
        m.AddRuntimeInstrumentation();    // GC, memory, threads, CPU
    };

    opts.ConfigureTracer = t =>
    {
        t.AddAspNetCoreInstrumentation(); // also gRPC server traces
    };

    opts.ConfigureLogging = log => { };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMigration();
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
