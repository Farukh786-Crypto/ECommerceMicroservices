using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
    public class CreateShoppingCartHandler : IRequestHandler<CreateShoppingCartCommand,ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly DiscountGrpcService _discountGrpcService;

        public CreateShoppingCartHandler(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService)
        {
            
            _basketRepository = basketRepository;
            _discountGrpcService = discountGrpcService;
        }
        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            //Apply discounts using GRPC call
            foreach (var item in request.items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price = item.Price - coupon.Amount;  // minus amount from discount amount
            }
            // convert command to domain entity
            var shoppingCartEntity = request.ToShoppingCartEntity();
            // save to redis
            var updateCart = await _basketRepository.UpsertBasket(shoppingCartEntity);
            // Convert back to response
            return updateCart.ToShoppingCartResponse();

        }
    }
}
