using DineFlow.Application.Common.Exceptions;
using DineFlow.Application.Common.Orders;
using MediatR;

namespace DineFlow.Application.Orders.Commands.PayOrder
{
    public class PayOrderCommandHandler : IRequestHandler<PayOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public PayOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task Handle(PayOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _orderRepository.GetById(request.OrderId)
                ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

            order.MarkPaid(request.ActorId);

            return Task.CompletedTask;
        }
    }
}
