

using Catalog.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.DTOs
{
    public record ProductDto
    (
        string id,
        string Name,
        string Summary,
        string Description,
        string ImageFile,
        BrandDTO Brand,
        TypeDTO Type,
        decimal Price,
        DateTimeOffset CreatedDate
    );

    public record BrandDTO(string Id,string Name);
    public record TypeDTO(string Id,string Name);

    public record class CreateProductDTO
    {
        [Required]
        public string? Name { get; init; }
        [Required]
        public string? Summary { get; init; }
        [Required]
        public string? Description { get; init; }
        [Required]
        public string? ImageFile { get; init; }
        [Required]
        public string? BrandId { get; init; }
        [Required]
        public string? TypeId { get; init; }
        [Range(0.01,double.MaxValue,ErrorMessage ="Price must be greater than 0")]
        public decimal Price { get; init; }
    }
    public record class UpdateProductDto
    {
        [Required]
        public string? Name { get; init; }
        [Required]
        public string? Summary { get; init; }
        [Required]
        public string? Description { get; init; }
        [Required]
        public string? ImageFile { get; init; }
        [Required]
        public string? BrandId { get; init; }
        [Required]
        public string? TypeId { get; init; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; init; }
    }
}
