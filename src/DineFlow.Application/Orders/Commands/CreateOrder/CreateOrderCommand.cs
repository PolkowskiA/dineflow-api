using DineFlow.Domain;
using MediatR;

namespace DineFlow.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(OrderType Type, List<CreateOrderItemDto> Items, Guid? TableId, Guid? ActorId = null) : IRequest<Guid>;
}