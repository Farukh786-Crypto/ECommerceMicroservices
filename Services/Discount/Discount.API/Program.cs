using Discount.API.Services;
using Discount.Application.Handlers;
using Discount.Core.IRepositories;
using Discount.Infrastructure.Repositories;
using Discount.Infrastructure.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Mediatr
var assemblies = new Assembly[]
    {
        Assembly.GetExecutingAssembly(), typeof(CreateDiscountHandler).Assembly
    };
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddGrpc();

// FIX: Enable both HTTP/1.1 and HTTP/2
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, o =>
    {
        o.Protocols = HttpProtocols.Http2;
    });
});

//Database Settings 
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//Migrate the database
app.MigrateDatabase();
app.UseRouting();
// Map gRPC service
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<DiscountService>();
});

// Optional test endpoint
app.MapGet("/", () => "Discount gRPC Service Running");

app.Run();
