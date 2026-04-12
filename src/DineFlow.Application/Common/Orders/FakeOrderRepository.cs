using DineFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Application.Common.Orders
{
    public class FakeOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new();

        public Order? GetById(Guid id)
            => _orders.FirstOrDefault(x => x.Id == id);

        public void Add(Order order)
            => _orders.Add(order);
    }
}