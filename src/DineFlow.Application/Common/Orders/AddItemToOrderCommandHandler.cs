using DineFlow.Application.Common.Products;
using DineFlow.Application.Orders.Commands.AddItemToOrder;
using DineFlow.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Application.Common.Orders
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
            var order = _orderRepository.GetById(request.OrderId)
                ?? throw new Exception("Order not found");

            var product = _productService.GetById(request.ProductId);

            var orderItem = new OrderItem(
                product.Id,
                product.Name,
                product.Price,
                request.Quantity
            );

            order.AddItem(orderItem);

            return Task.CompletedTask;
        }
    }
}