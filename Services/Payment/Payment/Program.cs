using EventBus.Messages.Common;
using MassTransit;
using Payment.EventBusConsumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// MassTransit
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<OrderCreatedConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.UseRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
        cfg.ReceiveEndpoint(EventBusConstant.OrderCreatedQueue, c =>
        {
            c.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
            c.ConfigureConsumer<OrderCreatedConsumer>(ctx);
        });
    });
});

//Register Logging
//builder.Host.UseSerilog(Logging.ConfigureLogger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
