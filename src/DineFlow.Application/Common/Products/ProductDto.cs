namespace DineFlow.Application.Common.Products
{
    public class ProductDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public decimal Price { get; init; }
    }
}