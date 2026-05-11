using MediatR;

namespace DineFlow.Application.Orders.Commands.AddItemToOrder
{
    public record AddItemToOrderCommand(
        Guid OrderId,
        Guid ProductId,
        int Quantity,
        Guid? ActorId = null) : IRequest;
}
