using System.Collections.Concurrent;

namespace DineFlow.Application.Common.Products
{
    public class FakeProductService : IProductService
    {
        public static readonly Guid BurgerId = Guid.Parse("6cfd89e6-0c26-42d1-9c38-f68f4d2f7d6f");
        public static readonly Guid FriesId = Guid.Parse("f1b8b2db-f754-4d5f-98f4-2c6d08dd7207");

        private readonly ConcurrentDictionary<Guid, ProductDto> _store = new();

        public FakeProductService()
        {
            AddProduct(BurgerId, "Burger", 10m);
            AddProduct(FriesId, "Fries", 5m);
        }

        public ProductDto GetById(Guid id)
        {
            if (_store.TryGetValue(id, out var product))
                return product;

            throw new KeyNotFoundException($"Product '{id}' not found.");
        }

        private void AddProduct(Guid id, string name, decimal price)
        {
            var product = new ProductDto { Id = id, Name = name, Price = price };

            _store[product.Id] = product;
        }
    }
}
