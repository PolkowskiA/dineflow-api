using DineFlow.Domain;
using DineFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

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
        public void Should_Be_Submitted_When_Items_Added_But_Not_Started()
        {
            var order = new Order(OrderType.Takeaway, null);
            order.AddItem(new OrderItem(Guid.NewGuid(), "Burger", 10, 1));

            order.GetStatus().Should().Be(OrderStatus.Submitted);
        }

        [Fact]
        public void Should_Be_InProgress_When_Any_Item_InPreparation()
        {
            var order = new Order(OrderType.Takeaway, null);
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);

            order.AddItem(item);

            var kitchenItem = item.KitchenItems.First();
            kitchenItem.StartPreparation();

            order.GetStatus().Should().Be(OrderStatus.InProgress);
        }

        [Fact]
        public void Should_Be_Completed_When_All_Items_Served()
        {
            var order = new Order(OrderType.Takeaway, null);
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 1);

            order.AddItem(item);

            var kitchenItem = item.KitchenItems.First();
            kitchenItem.StartPreparation();
            kitchenItem.MarkReady();
            kitchenItem.Serve();

            order.GetStatus().Should().Be(OrderStatus.Completed);
        }

        [Fact]
        public void Should_Be_Cancelled_When_All_Items_Cancelled()
        {
            var order = new Order(OrderType.Takeaway, null);
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 2);

            order.AddItem(item);

            foreach (var k in item.KitchenItems)
                k.Cancel(true);

            order.GetStatus().Should().Be(OrderStatus.Cancelled);
        }
    }
}