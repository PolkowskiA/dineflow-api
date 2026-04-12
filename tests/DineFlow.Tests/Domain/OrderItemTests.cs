using DineFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace DineFlow.Tests.Domain
{
    public class OrderItemTests
    {
        [Fact]
        public void Should_Create_KitchenItems_Based_On_Quantity()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 3);

            item.KitchenItems.Should().HaveCount(3);
        }

        [Fact]
        public void Should_Calculate_Total_Excluding_Cancelled()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 2);

            var kitchenItem = item.KitchenItems.First();
            kitchenItem.Cancel(true);

            item.GetTotal().Should().Be(10);
        }

        [Fact]
        public void Should_Be_AllServed_When_All_Items_Served_Or_Cancelled()
        {
            var item = new OrderItem(Guid.NewGuid(), "Burger", 10, 2);

            foreach (var k in item.KitchenItems)
            {
                k.StartPreparation();
                k.MarkReady();
                k.Serve();
            }

            item.AllServed().Should().BeTrue();
        }
    }
}