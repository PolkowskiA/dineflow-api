using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Application.Common.Products
{
    public class FakeProductService : IProductService
    {
        public ProductDto GetById(Guid id)
        {
            return new ProductDto
            {
                Id = id,
                Name = "Burger",
                Price = 10,
                CanCancelAfterStart = false
            };
        }
    }
}