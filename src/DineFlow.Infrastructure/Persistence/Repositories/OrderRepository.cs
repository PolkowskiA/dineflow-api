using DineFlow.Application.Common.Orders;
using DineFlow.Domain.Entities.Orders;
using DineFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DineFlow.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;

        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(
            Order order,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Orders.AddAsync(order, cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(
                    x => x.Id == orderId,
                    cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}