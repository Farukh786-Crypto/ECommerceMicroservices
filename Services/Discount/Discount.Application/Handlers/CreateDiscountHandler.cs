using Discount.Application.Commands;
using Discount.Application.DTOs;
using Discount.Application.Extensions;
using Discount.Application.Mappers;
using Discount.Core.IRepositories;
using Grpc.Core;
using MediatR;

namespace Discount.Application.Handlers
{
    public class CreateDiscountHandler : IRequestHandler<CreateDiscountCommand,CouponDto>
    {
        private readonly IDiscountRepository _discountRepository;
        public CreateDiscountHandler(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }
        public async Task<CouponDto> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            // Validation the input
            var validationErrorrs = new Dictionary<string,string>();
            if (string.IsNullOrWhiteSpace(request.ProductName))
                validationErrorrs["ProductName"] = "Product name must not be empty.";
            if(string.IsNullOrWhiteSpace(request.Description))
                validationErrorrs["Description"] = "Product Description must not be empty.";
            if(request.Amount<=0)
                validationErrorrs["Amount"] = "Amount must be greater than zero.";
            if(validationErrorrs.Any())
                throw GrpcErrorHelper.CreateValidationException(validationErrorrs);

            // Convert to Entity
            var coupon = request.ToEntity();

            // Save to Db
            var created = await _discountRepository.CreateDiscount(coupon);
            if(!created)
            {
                throw new RpcException(new Status(StatusCode.Internal,$"Could not create discount for product: {request.ProductName}"));
            }
            // Return DTO
            return coupon.ToDto();
        }
    }
}
