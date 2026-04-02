using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries
{
    public record GetProductByIdQuery(string id) : IRequest<ProductResponse>
    {
    }
}
