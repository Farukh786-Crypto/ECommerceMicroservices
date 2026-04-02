namespace Basket.Application.Responses
{
    public record class ShoppingCartItemResponse
    {
        public int Quantity { get; init; }
        public string? ImageFile { get; init; }
        public decimal Price { get; init; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
    }
}
