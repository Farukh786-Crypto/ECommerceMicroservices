using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.Entensions;
using Ordering.Application.EventBusConsumer;
using Ordering.Infrastructure.Dispatcher;
using Ordering.Infrastucture.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Ordering services coming from extensions
builder.Services.AddOrderingServices(builder.Configuration);

//Register Outbox Message Dispatcher
builder.Services.AddHostedService<OutboxMessageDispatcher>();

//Mass Transit 
builder.Services.AddMassTransit(config =>
{
    //Mark as Consumer
    config.AddConsumer<BasketOrderingConsumer>();
    config.AddConsumer<PaymentCompletedConsumer>();
    config.AddConsumer<PaymentFailedConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.UseRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
        //provide the queue name with consumer settings
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue, c =>
        {
            cfg.UseRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
            c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
        });
        cfg.ReceiveEndpoint(EventBusConstant.PaymentCompleteQueue, c =>
        {
            cfg.UseRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
            c.ConfigureConsumer<PaymentCompletedConsumer>(ctx);
        });
        cfg.ReceiveEndpoint(EventBusConstant.PaymentFailedQueue, c =>
        {
            cfg.UseRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
            c.ConfigureConsumer<PaymentFailedConsumer>(ctx);
        });
    });
});

//Register Logging
//builder.Host.UseSerilog(Logging.ConfigureLogger);
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

//Migration coming from dbExtension class
app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetRequiredService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger).Wait(); // program.cs is async and database is sync thats why we need to use Wait() here
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
