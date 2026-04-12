using System;
using System.Collections.Generic;
using System.Text;
using DineFlow.Domain;
using DineFlow.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DineFlow.Tests.Domain
{
    public class KitchenItemTests
    {
        [Fact]
        public void Should_Start_In_Pending_State()
        {
            var item = new KitchenItem();

            item.Status.Should().Be(OrderItemStatus.Pending);
        }

        [Fact]
        public void Should_Transition_Pending_To_InPreparation()
        {
            var item = new KitchenItem();

            item.StartPreparation();

            item.Status.Should().Be(OrderItemStatus.InPreparation);
        }

        [Fact]
        public void Should_Not_Allow_Invalid_Transition()
        {
            var item = new KitchenItem();

            Action act = () => item.MarkReady();

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_Follow_Full_Lifecycle()
        {
            var item = new KitchenItem();

            item.StartPreparation();
            item.MarkReady();
            item.Serve();

            item.Status.Should().Be(OrderItemStatus.Served);
        }

        [Fact]
        public void Should_Allow_Cancel_When_Allowed()
        {
            var item = new KitchenItem();

            item.Cancel(true);

            item.Status.Should().Be(OrderItemStatus.Cancelled);
        }

        [Fact]
        public void Should_Not_Allow_Cancel_When_Not_Allowed()
        {
            var item = new KitchenItem();

            Action act = () => item.Cancel(false);

            act.Should().Throw<InvalidOperationException>();
        }
    }
}