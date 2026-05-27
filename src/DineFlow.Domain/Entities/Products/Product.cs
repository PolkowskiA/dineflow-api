using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Domain.Entities.Products
{
    public class Product
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = default!;
        public decimal Price { get; private set; }

        public bool IsAvailable { get; private set; }

        private Product()
        {
        }

        public Product(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name is required", nameof(name));
            }

            if (price < 0)
            {
                throw new ArgumentException("Price cannot be negative", nameof(price));
            }

            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            IsAvailable = true;
        }

        public void ChangePrice(decimal price)
        {
            if (price < 0)
            {
                throw new ArgumentException("Price cannot be negative", nameof(price));
            }
        }

        public void Disable()
        {
            IsAvailable = false;
        }

        public void Enable()
        {
            IsAvailable = true;
        }
    }
}