using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Domain.Entities
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public OrderType Type { get; private set; }
        public Guid? TableId { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items;

        private Order()
        { }

        public Order(OrderType type, Guid? tableId)
        {
            if (type == OrderType.DineIn && tableId == null)
                throw new ArgumentException("TableId required for DineIn");

            if (type == OrderType.Takeaway && tableId != null)
                throw new ArgumentException("TableId must be null for Takeaway");

            Id = Guid.NewGuid();
            Type = type;
            TableId = tableId;
        }

        public void AddItem(OrderItem item)
        {
            if (GetStatus() != OrderStatus.Draft)
                throw new InvalidOperationException("Cannot add items after submission");

            _items.Add(item);
        }

        public OrderStatus GetStatus()
        {
            if (!_items.Any())
                return OrderStatus.Draft;

            if (_items.All(i => i.KitchenItems.All(k => k.Status == OrderItemStatus.Cancelled)))
                return OrderStatus.Cancelled;

            if (_items.All(i => i.AllServed()))
                return OrderStatus.Completed;

            if (_items.Any(i => i.AnyInProgress()))
                return OrderStatus.InProgress;

            return OrderStatus.Submitted;
        }

        public decimal GetTotal()
        {
            return _items.Sum(i => i.GetTotal());
        }
    }
}