using DineFlow.Application.Common.Orders;
using DineFlow.Application.Common.Products;
using DineFlow.Domain.Entities;
using DineFlow.Domain.Entities.OrderItems;
using MediatR;

namespace DineFlow.Application.Orders.Commands.AddItemToOrder
{
    public class AddItemToOrderCommandHandler : IRequestHandler<AddItemToOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public AddItemToOrderCommandHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task Handle(AddItemToOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(
                request.OrderId,
                cancellationToken);

            if (order is null)
            {
                throw new InvalidOperationException(
                    $"Order '{request.OrderId}' not found.");
            }

            var product = await _productRepository.GetByIdAsync(
                request.ProductId,
                cancellationToken);

            if (product is null)
            {
                throw new InvalidOperationException(
                    $"Product '{request.ProductId}' not found.");
            }

            if (!product.IsAvailable)
            {
                throw new InvalidOperationException(
                    $"Product '{product.Name}' is unavailable.");
            }

            var orderItem = new OrderItem(
                product.Id,
                product.Name,
                product.Price,
                request.Quantity);

            order.AddItem(orderItem);

            await _orderRepository.SaveChangesAsync(
                cancellationToken);
        }
    }
}