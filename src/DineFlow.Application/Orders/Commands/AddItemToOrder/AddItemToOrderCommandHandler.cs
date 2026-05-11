using DineFlow.Application.Common.Exceptions;
using DineFlow.Application.Common.Orders;
using DineFlow.Application.Common.Products;
using DineFlow.Domain.Entities;
using MediatR;

namespace DineFlow.Application.Orders.Commands.AddItemToOrder
{
    public class AddItemToOrderCommandHandler : IRequestHandler<AddItemToOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;

        public AddItemToOrderCommandHandler(
            IOrderRepository orderRepository,
            IProductService productService)
        {
            _orderRepository = orderRepository;
            _productService = productService;
        }

        public Task Handle(AddItemToOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _orderRepository.GetById(request.OrderId);
            if (order is null)
                throw new NotFoundException($"Order '{request.OrderId}' not found.");

            ProductDto product;
            try
            {
                product = _productService.GetById(request.ProductId);
            }
            catch (KeyNotFoundException)
            {
                throw new NotFoundException($"Product '{request.ProductId}' not found.");
            }

            var orderItem = new OrderItem(product.Id, product.Name, product.Price, request.Quantity);

            order.AddItem(orderItem, request.ActorId);

            return Task.CompletedTask;
        }
    }
}
