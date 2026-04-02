using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Abstractions;
using Ordering.Application.DTOs;
using Ordering.Application.Orders.CreateOrder;
using Ordering.Application.Orders.DeleteOrder;
using Ordering.Application.Orders.GetOrder;
using Ordering.Application.Orders.UpdateOrder;
using Ordering.Application.Mappers;

namespace Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ICommandHandler<CreateOrderCommand, int> _createOrderHandler;
        private readonly ICommandHandler<UpdateOrderCommand> _updateOrderHandler;
        private readonly ICommandHandler<DeleteOrderCommand> _deleteOrderHandler;
        private readonly IQueryHandler<GetOrderListQuery, List<OrderDto>> _getOrderListHandler;
        private readonly ILogger<OrderController> _logger;
        public OrderController(ICommandHandler<CreateOrderCommand, int> createOrderHandler, 
            ICommandHandler<UpdateOrderCommand> updateOrderHandler,
            ICommandHandler<DeleteOrderCommand> deleteOrderHandler,
            IQueryHandler<GetOrderListQuery, List<OrderDto>> getOrderListHandler,
            ILogger<OrderController> logger)
        {
            _createOrderHandler = createOrderHandler;
            _updateOrderHandler = updateOrderHandler;
            _deleteOrderHandler = deleteOrderHandler;
            _getOrderListHandler = getOrderListHandler;
            _logger = logger;
        }

        [HttpGet("{userName}",Name = "GetOrdersByUserName")]
        public async Task<ActionResult<List<OrderDto>>> GetOrdersByUserName(string userName,CancellationToken cancellationToken)
        {
            var query = new GetOrderListQuery(userName);
            var orders = await _getOrderListHandler.Handle(query,cancellationToken);
            _logger.LogInformation("Order fetched for user {@UserName}", userName);
            return Ok(orders);
        }

        // Testing purpose
        [HttpPost(Name="CheckoutOrder")]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CreateOrderDto dto , CancellationToken cancellationToken)
        {
            var command = dto.ToCommand();
            var orderId = await _createOrderHandler.Handle(command,cancellationToken);
            _logger.LogInformation("Order update with Id {OrderId}", orderId);
            return Ok(orderId);
        }

        [HttpPut(Name = "UpdateOrder")]
        public async Task<ActionResult> UpdateOrder([FromBody] OrderDto dto,CancellationToken cancellationToken)
        {
            var command = dto.ToCommand();
            await _updateOrderHandler.Handle(command,cancellationToken);
            _logger.LogInformation("Order Update with Id {OrderId}",dto.Id);
            return NoContent();
        }

        [HttpDelete("{id}",Name ="DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int id,CancellationToken cancellationToken)
        {
            var command = new DeleteOrderCommand(id);
            await _deleteOrderHandler.Handle(command,cancellationToken);
            _logger.LogInformation("Order delete with Id {OrderId}",id);
            return NoContent();
        }
    }
}
