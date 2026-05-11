using MediatR;

namespace DineFlow.Application.Orders.Commands.PayOrder
{
    public record PayOrderCommand(Guid OrderId, Guid? ActorId = null) : IRequest;
}
