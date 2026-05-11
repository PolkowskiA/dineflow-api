using DineFlow.Application.Common.Exceptions;
using DineFlow.Application.Common.Orders;
using DineFlow.Domain;
using DineFlow.Domain.Entities;
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

        public Task Handle(UpdateOrderItemStatusCommand request, CancellationToken cancellationToken)
        {
            var order = _orderRepository.GetById(request.OrderId)
                ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

            ApplyStatusChange(order, request);

            return Task.CompletedTask;
        }

        private static void ApplyStatusChange(Order order, UpdateOrderItemStatusCommand request)
        {
            switch (request.Status)
            {
                case OrderItemStatus.InPreparation:
                    order.StartItemPreparation(request.OrderItemId, request.Quantity, request.ActorId);
                    return;
                case OrderItemStatus.Ready:
                    order.MarkItemReady(request.OrderItemId, request.Quantity, request.ActorId);
                    return;
                case OrderItemStatus.Served:
                    order.ServeItem(request.OrderItemId, request.Quantity, request.ActorId);
                    return;
                case OrderItemStatus.Cancelled:
                    order.CancelItem(request.OrderItemId, request.Quantity, request.ActorId);
                    return;
                case OrderItemStatus.Pending:
                default:
                    throw new InvalidOperationException("Unsupported order item status transition");
            }
        }
    }
}
