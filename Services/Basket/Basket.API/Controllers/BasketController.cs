using Basket.Application.Commands;
using Basket.Application.DTOs;
using Basket.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IMediator mediator, ILogger<BasketController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        //Get: api/v1/basket/{userName}
        [HttpGet("{userName}")]
        public async Task<ActionResult<ShoppingCartDto>> GetBasket(string userName)
        {
            var query = new GetBasketByUserNameQuery(userName);
            var result = await _mediator.Send(query);
            _logger.LogInformation("Fetchinf Basket for {@userName}", userName);
            return Ok(result);
        }
        //Post: api/v1/basket
        [HttpPost]
        public async Task<ActionResult<ShoppingCartDto>> CreateOrUpdateBasket([FromBody] CreateShoppingCartCommand createShoppingCartCommand)
        {
            var result = await _mediator.Send(createShoppingCartCommand);
            return Ok(result);
        }
        //Delete: api/v1/basket/{userName}
        [HttpDelete("{userName}")]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            var cmd = new DeleteBasketByUserNameCommand(userName);
            await _mediator.Send(cmd);
            return Ok();
        }
        // Checkout
        [HttpPost("[action]")]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckoutDto dto)
        {
            await _mediator.Send(new BasketCheckoutCommand(dto));
            return Accepted();
        }
    }
}
