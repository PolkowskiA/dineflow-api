using MediatR;

namespace DineFlow.Application.Orders.Commands.SubmitOrder
{
    public record SubmitOrderCommand(Guid OrderId, Guid? ActorId = null) : IRequest;
}
