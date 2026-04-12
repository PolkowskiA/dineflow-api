using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Domain.Entities
{
    public class OrderItem
    {
        private readonly List<KitchenItem> _kitchenItems = new();

        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }

        public IReadOnlyCollection<KitchenItem> KitchenItems => _kitchenItems;

        private OrderItem()
        { }

        public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            Id = Guid.NewGuid();
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;

            for (int i = 0; i < quantity; i++)
            {
                _kitchenItems.Add(new KitchenItem());
            }
        }

        public decimal GetTotal()
        {
            return _kitchenItems
                .Where(x => x.Status != OrderItemStatus.Cancelled)
                .Count() * UnitPrice;
        }

        public bool AllServed() =>
            _kitchenItems.All(x => x.Status == OrderItemStatus.Served || x.Status == OrderItemStatus.Cancelled);

        public bool AnyInProgress() =>
            _kitchenItems.Any(x =>
                x.Status == OrderItemStatus.InPreparation ||
                x.Status == OrderItemStatus.Ready);
    }
}