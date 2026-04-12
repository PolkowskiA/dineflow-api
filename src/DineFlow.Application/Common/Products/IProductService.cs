using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Application.Common.Products
{
    public interface IProductService
    {
        ProductDto GetById(Guid id);
    }

    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public bool CanCancelAfterStart { get; set; }
    }
}