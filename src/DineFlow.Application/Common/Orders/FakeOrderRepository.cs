using System.Collections.Concurrent;
using DineFlow.Domain.Entities;

namespace DineFlow.Application.Common.Orders
{
    public class FakeOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<Guid, Order> _orders = new();

        public Order? GetById(Guid id)
            => _orders.TryGetValue(id, out var order) ? order : null;

        public void Add(Order order)
            => _orders[order.Id] = order;
    }
}
