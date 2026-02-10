using BuildingBlocks.Observability;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// add services to the container
var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config =>
{
    //automatically find and register all handlers for commands, queries from the current assembly
    //where the Program class is located. 
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

//scan assembly for any validators and register them
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);
}).UseLightweightSessions();//for performance

// feel seeds data
if(builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

// global custom exception handler
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

// Add OTEL(OpenTelemetry) 
builder.Services.AddObservability("Catalog.API", builder.Configuration, opts =>
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

        // PostgreSQL (Marten / Npgsql)
        t.AddNpgsql();
    };

    opts.ConfigureLogging = log => { };
});
var app = builder.Build();

// configure the HTTP request pipeline
app.MapCarter();

// configure the app to use custom exception handler
app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
