var builder = WebApplication.CreateBuilder(args);

//add services to the container
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

//configure the HTTP request pipeline
app.MapReverseProxy();

app.Run();
