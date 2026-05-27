using DineFlow.Application.Common.Orders;
using DineFlow.Application.Common.Products;
using DineFlow.Domain.Entities.OrderItems;
using DineFlow.Domain.Entities.Orders;
using MediatR;

namespace DineFlow.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler
     : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.Type, request.TableId);

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(
                    item.ProductId,
                    cancellationToken);

                if (product is null)
                    throw new InvalidOperationException(
                        $"Product '{item.ProductId}' not found.");

                if (!product.IsAvailable)
                    throw new InvalidOperationException(
                        $"Product '{product.Name}' is unavailable.");

                var orderItem = new OrderItem(
                    product.Id,
                    product.Name,
                    product.Price,
                    item.Quantity);

                order.AddItem(orderItem);
            }

            order.Submit();

            await _orderRepository.AddAsync(
                order,
                cancellationToken);

            await _orderRepository.SaveChangesAsync(
                cancellationToken);

            return order.Id;
        }
    }
}