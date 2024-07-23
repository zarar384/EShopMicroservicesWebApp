using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;

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
    opts.Schema.For<ShoppingCart>().Identity(x=>x.UserName);
}).UseLightweightSessions();//for performance

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CashedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// configure the HTTP request pipeline
app.MapCarter();
app.UseExceptionHandler(options => { });

app.Run();
