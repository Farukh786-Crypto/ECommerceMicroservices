using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Application.Abstractions;
using Ordering.Application.Mappers;
using Ordering.Application.Orders.CreateOrder;

namespace Ordering.Application.EventBusConsumer
{
    // after event called create basket then we delete basket after checkout event
    // here we consume it BasketCheckoutEvent when basketcheckout it called this method handle
    public class BasketOrderingConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly ICommandHandler<CreateOrderCommand, int> _createOrderHandler;
        private readonly ILogger<BasketOrderingConsumer> _logger;

        public BasketOrderingConsumer(ICommandHandler<CreateOrderCommand,int> createOrderHandler,ILogger<BasketOrderingConsumer> logger)
        {
            _createOrderHandler = createOrderHandler;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            using var scopr = _logger.BeginScope("Consuming Basket Checkout Event for {CorrelationId}",context.Message.CorrelationId);
            var command = context.Message.ToCheckoutOrderCommand();
            // after basketChekoutEvent called create basket called here
            var orderId = await _createOrderHandler.Handle(command,context.CancellationToken);
            _logger.LogInformation("Basket checkout Event Completed Successfully !! OrderId: {OrderId}",orderId);
        }
    }
}
