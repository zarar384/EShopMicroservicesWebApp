using BuildingBlocks.Observability;
using Discount.Grpc.Data;
using Discount.Grpc.Services;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Logs;
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

        t.AddSqlClientInstrumentation(o =>
        {
            o.RecordException = true; // record exceptions in traces

            // exclude noise SQL statements from traces (e.g. "SELECT 1", "PRAGMA user_version", etc.)
            //o.Filter = (cmd) =>
            //{
            //    if(cmd is not System.Data.Common.DbCommand db)
            //        return false;
            //    
            //    // PRAGMA.. statements are used by EF Core to check database compatibility and are not useful for tracing
            //    return !db.CommandText.StartsWith("SELECT 1", StringComparison.OrdinalIgnoreCase);
            //};
        });
    };

    opts.ConfigureLogging = log => {

        // exclude logs below Warning level to reduce noise in log storage (optional)
        // log.AddProcessor(new LogLevelFilterProcessor(LogLevel.Warning));
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMigration();
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
