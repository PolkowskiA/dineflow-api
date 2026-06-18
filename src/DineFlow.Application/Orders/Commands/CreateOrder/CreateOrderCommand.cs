using DineFlow.Domain;
using MediatR;

namespace DineFlow.Application.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderCommand(
        OrderType Type,
        Guid? TableId,
        Guid? ActorId = null)
        : IRequest<Guid>;
}