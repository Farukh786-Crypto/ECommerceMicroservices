using Catalog.Application.Responses;
using Catalog.Core.Entities;

namespace Catalog.Application.Mappers
{
    public static class TypeMapper
    {
        public static TypesResponse ToResponse(this ProductType productType)
        {
            return new TypesResponse
            {
                Id = productType.Id,
                Name = productType.Name,
            };
        }
        public static IList<TypesResponse> ToResponseList(this IEnumerable<ProductType> types)
        {
            return types.Select(t=>t.ToResponse()).ToList();
        }
    }
}
