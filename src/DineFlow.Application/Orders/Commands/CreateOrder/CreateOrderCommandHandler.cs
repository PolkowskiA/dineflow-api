using DineFlow.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private static readonly List<Order> _orders = new();

        public Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.Type, request.TableId);

            _orders.Add(order);

            return Task.FromResult(order.Id);
        }
    }
}