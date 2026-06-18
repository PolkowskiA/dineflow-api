using DineFlow.Application.Common.Orders;
using DineFlow.Domain;
using MediatR;

namespace DineFlow.Application.Orders.Commands.UpdateOrderItemStatus
{
    public class UpdateOrderItemStatusCommandHandler : IRequestHandler<UpdateOrderItemStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderItemStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(UpdateOrderItemStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                throw new InvalidOperationException(
                    $"Order '{request.OrderId}' not found.");
            }

            switch (request.Status)
            {
                case OrderItemStatus.InPreparation:
                    order.StartItemPreparation(
                        request.OrderItemId,
                        request.Quantity);
                    break;

                case OrderItemStatus.Ready:
                    order.MarkItemReady(
                        request.OrderItemId,
                        request.Quantity);
                    break;

                case OrderItemStatus.Served:
                    order.ServeItem(
                        request.OrderItemId,
                        request.Quantity);
                    break;

                case OrderItemStatus.Cancelled:
                    order.CancelItem(
                        request.OrderItemId,
                        request.Quantity);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported status transition '{request.Status}'.");
            }

            await _orderRepository.SaveChangesAsync(
                cancellationToken);
        }
    }
}