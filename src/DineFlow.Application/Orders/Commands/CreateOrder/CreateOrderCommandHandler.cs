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

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.Type, request.TableId, request.ActorId);

            await _orderRepository.AddAsync(
                order,
                cancellationToken);

            await _orderRepository.SaveChangesAsync(
                cancellationToken);

            return order.Id;
        }
    }
}