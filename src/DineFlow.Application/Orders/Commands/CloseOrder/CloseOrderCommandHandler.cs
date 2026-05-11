using DineFlow.Application.Common.Exceptions;
using DineFlow.Application.Common.Orders;
using MediatR;

namespace DineFlow.Application.Orders.Commands.CloseOrder
{
    public class CloseOrderCommandHandler : IRequestHandler<CloseOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public CloseOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task Handle(CloseOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _orderRepository.GetById(request.OrderId)
                ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

            order.Close(request.ActorId);

            return Task.CompletedTask;
        }
    }
}
