using DineFlow.Application.Common.Products;
using DineFlow.Domain.Entities.Products;
using DineFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .FirstOrDefaultAsync(
                    x => x.Id == productId,
                    cancellationToken);
        }
    }
}