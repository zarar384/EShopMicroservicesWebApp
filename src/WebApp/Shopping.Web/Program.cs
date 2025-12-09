using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    });
builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    });
builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    });

// Add OTEL(OpenTelemetry) 
var otlpUrl = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!;

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("Shopping.Web")) // service name
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()  // HTTP request metrics
        .AddHttpClientInstrumentation() // Outgoing HTTP calls metrics
        .AddRuntimeInstrumentation() // GC, memory, threads, CPU
        .AddOtlpExporter(e => // send metrics to OTLP(OTEL Protocol)
        {
            e.Endpoint = new Uri(otlpUrl);
            e.Protocol = OtlpExportProtocol.Grpc;
        }))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(e =>
        {
            e.Endpoint = new Uri(otlpUrl);
            e.Protocol = OtlpExportProtocol.Grpc;
        }));

builder.Logging.ClearProviders(); // replace default logging with OTL logging
builder.Logging.AddOpenTelemetry(o =>
{
    o.IncludeFormattedMessage = true; // render log message
    o.IncludeScopes = true; 
    o.ParseStateValues = true;
    o.AddOtlpExporter(e => // send to OTLP
    {
        e.Endpoint = new Uri(otlpUrl);
        e.Protocol = OtlpExportProtocol.Grpc;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
