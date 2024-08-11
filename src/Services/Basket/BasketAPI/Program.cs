using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Discount.Grps;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

//add services to the container
//Application Services
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

//Data Services
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);
    opts.Schema.For<ShoppingCart>().Identity(x=>x.UserName);
}).UseLightweightSessions();//for performance

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CashedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

//Grpc Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(opts =>
{
    opts.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})
  .ConfigurePrimaryHttpMessageHandler(() =>
  {
      //accept any server certificate validators in gRPC settings
      //!ONLY for development purpose
      var handler = new HttpClientHandler
      {
          ServerCertificateCustomValidationCallback =
          HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
      };

      return handler;
  });

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("RedisConnection")!);

var app = builder.Build();

// configure the HTTP request pipeline
app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health",
    new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
