using EventBus.Messages.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ordering.Infrastucture.Data;


namespace Ordering.Infrastructure.Dispatcher
{   // it is background service read pendig event from database and publish into message broker i.e rabbitMQ to ensure no event loss
    // when basket checkout order select then go to payment if payment failed then all rollback this tracsaction  
    public class OutboxMessageDispatcher : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxMessageDispatcher> _logger;
        public OutboxMessageDispatcher(IServiceProvider serviceProvider, ILogger<OutboxMessageDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<OrderContext>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var pendingMessage = await dbContext.OutboxMessage
                                       .Where(x => x.ProcessedOn == null)
                                       .OrderBy(x => x.OccuredOn)
                                       .Take(20)
                                       .ToListAsync();
                foreach (var message in pendingMessage)
                {
                    try
                    {
                        var orderCreatedEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(message.Content);
                        await publishEndpoint.Publish(orderCreatedEvent);
                        message.ProcessedOn = DateTime.UtcNow;
                        _logger.LogInformation("Published outbox message {Id}", message.Id);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex,"Failed to publish outBox Message {Id}",message.Id);
                    }
                }
                await dbContext.SaveChangesAsync(stoppingToken);
                await Task.Delay(10000,stoppingToken); // when too much records then give some time
            }
        }
    }
}
