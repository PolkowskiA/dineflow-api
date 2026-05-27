using DineFlow.Application.Common.Exceptions;
using DineFlow.Application.Common.Orders;
using MediatR;

namespace DineFlow.Application.Orders.Commands.SubmitOrder
{
    public class SubmitOrderCommandHandler
     : IRequestHandler<SubmitOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public SubmitOrderCommandHandler(
            IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(
            SubmitOrderCommand request,
            CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(
                request.OrderId,
                cancellationToken);

            if (order is null)
            {
                throw new InvalidOperationException(
                    $"Order '{request.OrderId}' not found.");
            }

            order.Submit();

            await _orderRepository.SaveChangesAsync(
                cancellationToken);
        }
    }
}