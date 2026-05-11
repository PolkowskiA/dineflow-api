using MediatR;

namespace DineFlow.Application.Orders.Commands.CloseOrder
{
    public record CloseOrderCommand(Guid OrderId, Guid? ActorId = null) : IRequest;
}
