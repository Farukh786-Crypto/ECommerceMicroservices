using Basket.Application.DTOs;
using MediatR;

namespace Basket.Application.Commands
{
    // Basket: BasketCheckoutCommand → returns Unit (nothing!)
    public record BasketCheckoutCommand(BasketCheckoutDto BasketCheckoutDto) : IRequest<Unit>;

}
