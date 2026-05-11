using DineFlow.Domain;
using MediatR;

namespace DineFlow.Application.Orders.Commands.UpdateOrderItemStatus
{
    public record UpdateOrderItemStatusCommand(
        Guid OrderId,
        Guid OrderItemId,
        OrderItemStatus Status,
        int Quantity = 1,
        Guid? ActorId = null) : IRequest;
}
