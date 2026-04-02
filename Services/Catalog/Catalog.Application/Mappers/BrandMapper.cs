using Catalog.Application.Responses;
using Catalog.Core.Entities;

namespace Catalog.Application.Mappers
{
    /// <summary>
    /// Mapper class responsible for converting
    /// Entity objects (Core layer) into Response DTOs (Application layer).
    /// 
    /// Why?
    /// We never expose database entities directly to API responses.
    /// </summary>
    public static class BrandMapper
    {
        /// <summary>
        /// Converts a collection of ProductBrand entities
        /// into a list of BrandResponse DTOs.
        ///
        /// IEnumerable<ProductBrand> allows List, Array, or any collection type.
        /// </summary>
        /// <param name="brands">Collection of ProductBrand entities</param>
        /// <returns>List of BrandResponse DTOs</returns>
        public static BrandResponse ToResponse(this ProductBrand brand)
        {
            // Create new response object and map required properties
            // Only expose fields needed by API client
            return new BrandResponse
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }
        /// <summary>
        /// Mapper class responsible for converting
        /// Entity objects (Core layer) into Response DTOs (Application layer).
        /// 
        /// Why?
        /// We never expose database entities directly to API responses.
        /// </summary>
        public static IList<BrandResponse> ToResponseList(this IEnumerable<ProductBrand> brands)
        {
           
            // Select() loops through each brand
            // b.ToResponse() converts each entity to DTO
            // ToList() materializes result into a list
            return brands.Select(b=>b.ToResponse()).ToList();
        }
    }
}
