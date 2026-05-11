using DineFlow.Domain;
using DineFlow.Domain.Entities;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DineFlow.Tests.Domain
{
    public class OrderItemTests
    {
        [Fact]
        public void Should_Start_In_Pending_State()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 3);

            item.Status.Should().Be(OrderItemStatus.Pending);
        }

        [Fact]
        public void Should_Keep_Product_Snapshot_Data()
        {
            var productId = Guid.NewGuid();

            var item = new OrderItem(productId, "Burger", 10, 2);

            item.ProductId.Should().Be(productId);
            item.ProductName.Should().Be("Burger");
            item.UnitPrice.Should().Be(10);
            item.Quantity.Should().Be(2);
        }

        [Fact]
        public void Should_Calculate_Total_From_Quantity_And_Snapshot_Price()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 2);

            item.GetTotal().Should().Be(20);
        }

        [Fact]
        public void Should_Follow_Full_Lifecycle()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);

            item.StartPreparation(1);
            item.MarkReady(1);
            item.Serve(1);

            item.Status.Should().Be(OrderItemStatus.Served);
        }

        [Fact]
        public void Should_Support_Partial_Quantity_Transitions()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 3);

            item.StartPreparation(2);
            item.MarkReady(1);
            item.Serve(1);

            item.InPreparationQuantity.Should().Be(1);
            item.ReadyQuantity.Should().Be(0);
            item.ServedQuantity.Should().Be(1);
            item.PendingQuantity.Should().Be(1);
            item.Status.Should().Be(OrderItemStatus.InPreparation);
        }

        [Fact]
        public void Should_Not_Allow_Backward_Transition()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);
            item.StartPreparation(1);

            var act = () => item.Serve(1);

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Allow_Cancel_When_Pending()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);

            item.Cancel(1);

            item.Status.Should().Be(OrderItemStatus.Cancelled);
            item.GetTotal().Should().Be(0);
        }

        [Fact]
        public void Should_Allow_Cancel_When_InPreparation()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);
            item.StartPreparation(1);

            item.Cancel(1);

            item.Status.Should().Be(OrderItemStatus.Cancelled);
        }

        [Fact]
        public void Should_Not_Allow_Cancel_When_Ready()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);
            item.StartPreparation(1);
            item.MarkReady(1);

            var act = () => item.Cancel(1);

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Calculate_Total_For_Active_Quantity()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 3);

            item.Cancel(1);

            item.GetTotal().Should().Be(20);
        }

        [Fact]
        public void Should_Cancel_From_Pending_And_InPreparation_Quantities()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 3);
            item.StartPreparation(2);

            item.Cancel(3);

            item.PendingQuantity.Should().Be(0);
            item.InPreparationQuantity.Should().Be(0);
            item.CancelledQuantity.Should().Be(3);
            item.Status.Should().Be(OrderItemStatus.Cancelled);
        }

        [Fact]
        public void Should_Not_Start_More_Than_Pending_Quantity()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 2);

            var act = () => item.StartPreparation(3);

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Not_MarkReady_More_Than_InPreparation_Quantity()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 2);
            item.StartPreparation(1);

            var act = () => item.MarkReady(2);

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Not_Serve_More_Than_Ready_Quantity()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 2);
            item.StartPreparation(1);
            item.MarkReady(1);

            var act = () => item.Serve(2);

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Record_Last_Modifying_Actor()
        {
            var actorId = Guid.NewGuid();
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);

            item.StartPreparation(1, actorId);

            item.LastModifiedBy.Should().Be(actorId);
            item.LastModifiedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Expose_RowVersion_For_Optimistic_Concurrency_Mapping()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);

            item.RowVersion.Should().NotBeNull();
        }

        [Fact]
        public void Should_Mark_RowVersion_With_Timestamp_Attribute()
        {
            var property = typeof(OrderItem).GetProperty(nameof(OrderItem.RowVersion));

            property.Should().NotBeNull();
            property!.GetCustomAttribute<TimestampAttribute>().Should().NotBeNull();
        }
    }
}
