using DineFlow.Domain;
using DineFlow.Domain.Common.Exceptions;
using DineFlow.Domain.Entities.Orders;
using DineFlow.Domain.Entities.OrderItems;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DineFlow.Tests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void Should_Start_As_Draft_When_No_Items()
        {
            var order = new Order(OrderType.Takeaway, null);

            order.GetStatus().Should().Be(OrderStatus.Draft);
        }

        [Fact]
        public void Should_Stay_Draft_When_Items_Are_Added_Before_Submit()
        {
            var order = new Order(OrderType.Takeaway, null);
            order.AddItem(new OrderItem(Guid.NewGuid(), "Burger", 10, 1));
            order.AddItem(new OrderItem(Guid.NewGuid(), "Fries", 5, 1));

            order.GetStatus().Should().Be(OrderStatus.Draft);
            order.Items.Should().HaveCount(2);
        }

        [Fact]
        public void Should_Not_Submit_Empty_Order()
        {
            var order = new Order(OrderType.Takeaway, null);

            var act = () => order.Submit();

            act.Should().Throw<OrderCannotBeSubmittedException>();
        }

        [Fact]
        public void Should_Be_Submitted_When_Order_Is_Submitted()
        {
            var order = CreateOrderWithItem();

            order.Submit();

            order.GetStatus().Should().Be(OrderStatus.Submitted);
        }

        [Fact]
        public void Should_Not_Add_Items_After_Submit()
        {
            var order = CreateOrderWithItem();
            order.Submit();

            var act = () => order.AddItem(new OrderItem(Guid.NewGuid(), "Fries", 5, 1));

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Be_InProgress_When_Any_Item_Is_InPreparation()
        {
            var order = CreateOrderWithItem();
            var item = order.Items.First();

            order.Submit();
            order.StartItemPreparation(item.Id, 1);

            order.GetStatus().Should().Be(OrderStatus.InProgress);
        }

        [Fact]
        public void Should_Be_Completed_When_All_Active_Items_Are_Served()
        {
            var order = CreateOrderWithItem();
            var item = order.Items.First();

            order.Submit();
            order.StartItemPreparation(item.Id, 1);
            order.MarkItemReady(item.Id, 1);
            order.ServeItem(item.Id, 1);

            order.GetStatus().Should().Be(OrderStatus.Completed);
        }

        [Fact]
        public void Should_Not_Be_Completed_When_Item_Is_Partially_Served()
        {
            var order = new Order(OrderType.Takeaway, null);
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 2);
            order.AddItem(item);

            order.Submit();
            order.StartItemPreparation(item.Id, 2);
            order.MarkItemReady(item.Id, 2);
            order.ServeItem(item.Id, 1);

            order.GetStatus().Should().Be(OrderStatus.InProgress);
        }

        [Fact]
        public void Should_Be_Completed_When_All_Active_Quantities_Are_Served()
        {
            var order = new Order(OrderType.Takeaway, null);
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 3);
            order.AddItem(item);

            order.Submit();
            order.CancelItem(item.Id, 1);
            order.StartItemPreparation(item.Id, 2);
            order.MarkItemReady(item.Id, 2);
            order.ServeItem(item.Id, 2);

            order.GetStatus().Should().Be(OrderStatus.Completed);
            order.GetTotal().Should().Be(20);
        }

        [Fact]
        public void Should_Not_Be_Paid_When_Not_Completed()
        {
            var order = CreateOrderWithItem();
            order.Submit();

            var act = () => order.MarkPaid();

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Follow_Completed_Paid_Closed_Lifecycle()
        {
            var order = CreateCompletedOrder();

            order.MarkPaid();
            order.GetStatus().Should().Be(OrderStatus.Paid);

            order.Close();
            order.GetStatus().Should().Be(OrderStatus.Closed);
        }

        [Fact]
        public void Should_Be_Cancelled_When_All_Items_Are_Cancelled()
        {
            var order = CreateOrderWithItem();
            var item = order.Items.First();

            order.CancelItem(item.Id, 1);

            order.GetStatus().Should().Be(OrderStatus.Cancelled);
        }

        [Fact]
        public void Should_Record_Created_And_Last_Modifying_Actor()
        {
            var creatorId = Guid.NewGuid();
            var submitterId = Guid.NewGuid();
            var order = new Order(OrderType.Takeaway, null, creatorId);
            order.AddItem(new OrderItem(Guid.NewGuid(), "Burger", 10, 1), creatorId);

            order.Submit(submitterId);

            order.CreatedBy.Should().Be(creatorId);
            order.LastModifiedBy.Should().Be(submitterId);
            order.LastModifiedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Expose_RowVersion_For_Optimistic_Concurrency_Mapping()
        {
            var order = new Order(OrderType.Takeaway, null);

            order.RowVersion.Should().NotBeNull();
        }

        [Fact]
        public void Should_Mark_RowVersion_With_Timestamp_Attribute()
        {
            var property = typeof(Order).GetProperty(nameof(Order.RowVersion));

            property.Should().NotBeNull();
            property!.GetCustomAttribute<TimestampAttribute>().Should().NotBeNull();
        }

        [Fact]
        public void Should_Be_InProgress_When_Item_Is_Partially_Served()
        {
            var order = new Order(OrderType.Takeaway, null);
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 3);

            order.AddItem(item);

            order.Submit();

            order.StartItemPreparation(item.Id, 3);
            order.MarkItemReady(item.Id, 3);
            order.ServeItem(item.Id, 1);

            order.GetStatus().Should().Be(OrderStatus.InProgress);
        }

        [Fact]
        public void Should_Not_Submit_Order_With_Only_Cancelled_Items()
        {
            var order = new Order(OrderType.Takeaway, null);

            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);

            order.AddItem(item);
            order.CancelItem(item.Id);

            var act = () => order.Submit();

            act.Should()
                .Throw<OrderCannotBeSubmittedException>();
        }

        [Fact]
        public void Should_Not_Cancel_Item_After_Order_Is_Paid()
        {
            var order = CreateCompletedOrder();
            var item = order.Items.First();

            order.MarkPaid();

            var act = () => order.CancelItem(item.Id);

            act.Should().Throw<InvalidOperationException>();
        }

        private static Order CreateOrderWithItem()
        {
            var order = new Order(OrderType.Takeaway, null);
            order.AddItem(new OrderItem(Guid.NewGuid(), "Burger", 10, 1));

            return order;
        }

        private static Order CreateCompletedOrder()
        {
            var order = CreateOrderWithItem();
            var item = order.Items.First();

            order.Submit();
            order.StartItemPreparation(item.Id, 1);
            order.MarkItemReady(item.Id, 1);
            order.ServeItem(item.Id, 1);

            return order;
        }
    }
}