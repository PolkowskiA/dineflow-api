using DineFlow.Application.Common.Orders;
using DineFlow.Domain.Entities;
using MediatR;

namespace DineFlow.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.Type, request.TableId, request.ActorId);

            _orderRepository.Add(order);

            return Task.FromResult(order.Id);
        }
    }
}
