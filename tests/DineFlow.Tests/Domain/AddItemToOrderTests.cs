using DineFlow.Application.Common.Orders;
using DineFlow.Application.Common.Products;
using DineFlow.Application.Orders.Commands.AddItemToOrder;
using DineFlow.Domain;
using DineFlow.Domain.Entities;
using FluentAssertions;

namespace DineFlow.Tests.Domain
{
    public class AddItemToOrderTests
    {
        [Fact]
        public async Task Should_Add_Item_To_Order()
        {
            var repo = new FakeOrderRepository();
            var productService = new FakeProductService();

            var order = new Order(OrderType.Takeaway, null);
            repo.Add(order);

            var handler = new AddItemToOrderCommandHandler(repo, productService);

            var command = new AddItemToOrderCommand(order.Id, Guid.NewGuid(), 2);

            await handler.Handle(command, default);

            order.Items.Should().HaveCount(1);
        }
    }
}