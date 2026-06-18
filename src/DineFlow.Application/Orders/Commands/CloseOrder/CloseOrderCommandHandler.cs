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

        public async Task Handle(CloseOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(
                request.OrderId,
                cancellationToken);

            if (order is null)
            {
                throw new InvalidOperationException(
                    $"Order '{request.OrderId}' not found.");
            }

            order.Close();

            await _orderRepository.SaveChangesAsync(
                cancellationToken);
        }
    }
}