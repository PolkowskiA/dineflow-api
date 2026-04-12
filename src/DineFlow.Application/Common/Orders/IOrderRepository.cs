using DineFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Application.Common.Orders
{
    public interface IOrderRepository
    {
        Order? GetById(Guid id);

        void Add(Order order);
    }
}