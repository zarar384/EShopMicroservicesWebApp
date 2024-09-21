using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

/*add services to the container

Infrastructure - EF Core
Application - MediatR
API - Carter, HealthChecks, ...
*/

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices();

var app = builder.Build();

//configure the HTTP request pipeline

app.Run();
