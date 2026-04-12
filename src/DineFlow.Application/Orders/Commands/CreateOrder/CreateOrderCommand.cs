using System;
using System.Collections.Generic;
using System.Text;
using DineFlow.Domain;
using MediatR;

namespace DineFlow.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(OrderType Type, Guid? TableId) : IRequest<Guid>;
}