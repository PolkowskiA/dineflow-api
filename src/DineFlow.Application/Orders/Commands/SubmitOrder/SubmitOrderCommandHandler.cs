using DineFlow.Application.Common.Exceptions;
using DineFlow.Application.Common.Orders;
using MediatR;

namespace DineFlow.Application.Orders.Commands.SubmitOrder
{
    public class SubmitOrderCommandHandler : IRequestHandler<SubmitOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public SubmitOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task Handle(SubmitOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _orderRepository.GetById(request.OrderId)
                ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

            order.Submit(request.ActorId);

            return Task.CompletedTask;
        }
    }
}
