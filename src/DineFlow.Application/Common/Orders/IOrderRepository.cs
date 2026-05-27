using DineFlow.Domain.Entities;
using DineFlow.Domain.Entities.Orders;

namespace DineFlow.Application.Common.Orders
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken = default);

        Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}