using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DineFlow.Application.Orders.Commands.AddItemToOrder
{
    public record AddItemToOrderCommand(
        Guid OrderId,
        Guid ProductId,
        int Quantity
    ) : IRequest;
}