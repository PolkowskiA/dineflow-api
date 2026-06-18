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

        public async Task Handle(PayOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(
                request.OrderId,
                cancellationToken);

            if (order is null)
            {
                throw new InvalidOperationException(
                    $"Order '{request.OrderId}' not found.");
            }

            order.MarkPaid();

            await _orderRepository.SaveChangesAsync(
                cancellationToken);
        }
    }
}