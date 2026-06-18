using DineFlow.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Application.Common.Products
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);
    }
}