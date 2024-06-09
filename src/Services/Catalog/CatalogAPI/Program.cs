var builder = WebApplication.CreateBuilder(args);

// add services to the container
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    //automatically find and register all handlers for commands, queries from the current assembly
    //where the Program class is located. 
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

// configure the HTTP request pipeline
app.MapCarter();

app.Run();
