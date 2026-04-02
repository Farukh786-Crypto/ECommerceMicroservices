using Discount.Application.Commands;
using Discount.Application.DTOs;
using Discount.Application.Extensions;
using Discount.Application.Mappers;
using Discount.Core.IRepositories;
using Grpc.Core;
using MediatR;

namespace Discount.Application.Handlers
{
    public class UpdateDiscountHandler : IRequestHandler<UpdateDiscountCommand,CouponDto>
    {
        private readonly IDiscountRepository _discountRepository;
        public UpdateDiscountHandler(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }
        public async Task<CouponDto> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            // Validation the input
            var validationErrors = new Dictionary<string,string>();
            if (string.IsNullOrWhiteSpace(request.ProductName))
                validationErrors["ProductName"] = "Product name must not be empty.";
            if (string.IsNullOrWhiteSpace(request.Description))
                validationErrors["Description"] = "Product Description must not be empty.";
            if (request.Amount <= 0)
                validationErrors["Amount"] = "Amount must be greater than zero.";
            if (validationErrors.Any())
                throw GrpcErrorHelper.CreateValidationException(validationErrors);

            // Convert to Entity
            var coupon = request.ToEntity();

            // Update
            var updated = await _discountRepository.UpdateDiscount(coupon);
            if(!updated)
            {
                throw new RpcException(new Status(StatusCode.Internal,$"Could not updated discount for product : {request.ProductName}"));
            }
            // return DTO
            return coupon.ToDto();
        }
    }
}
