using DineFlow.Domain.Entities;

namespace DineFlow.Application.Common.Orders
{
    public interface IOrderRepository
    {
        Order? GetById(Guid id);

        void Add(Order order);
    }
}