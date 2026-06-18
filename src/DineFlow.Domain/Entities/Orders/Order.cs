using DineFlow.Domain.Common.Exceptions;
using DineFlow.Domain.Entities.OrderItems;
using System.ComponentModel.DataAnnotations;

namespace DineFlow.Domain.Entities.Orders
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public OrderType Type { get; private set; }
        public Guid? TableId { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? SubmittedAt { get; private set; }
        public DateTimeOffset? PaidAt { get; private set; }
        public DateTimeOffset? ClosedAt { get; private set; }
        public Guid? CreatedBy { get; private set; }
        public Guid? LastModifiedBy { get; private set; }
        public DateTimeOffset? LastModifiedAt { get; private set; }

        [Timestamp]
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        public IReadOnlyCollection<OrderItem> Items => _items;

        private bool IsSubmitted => SubmittedAt.HasValue;
        private bool IsPaid => PaidAt.HasValue;
        private bool IsClosed => ClosedAt.HasValue;

        private Order()
        {
        }

        public Order(OrderType type, Guid? tableId, Guid? createdBy = null)
        {
            if (type == OrderType.DineIn && tableId is null)
                throw new ArgumentException("TableId required for DineIn", nameof(tableId));

            if (type == OrderType.Takeaway && tableId is not null)
                throw new ArgumentException("TableId must be null for Takeaway", nameof(tableId));

            Id = Guid.NewGuid();
            Type = type;
            TableId = tableId;
            CreatedAt = DateTimeOffset.UtcNow;
            CreatedBy = createdBy;
            Touch(createdBy);
        }

        public void AddItem(OrderItem item, Guid? actorId = null)
        {
            if (IsSubmitted)
                throw new InvalidOperationException("Cannot add items after submission");

            _items.Add(item);
            Touch(actorId);
        }

        public void Submit(Guid? actorId = null)
        {
            if (IsSubmitted)
                throw new InvalidOperationException("Order already submitted");

            if (!_items.Any())
                throw new OrderCannotBeSubmittedException("Order must contain at least one item");

            if (_items.All(i => i.Status == OrderItemStatus.Cancelled))
                throw new OrderCannotBeSubmittedException("Order must contain at least one active item");

            SubmittedAt = DateTimeOffset.UtcNow;
            Touch(actorId);
        }

        public void StartItemPreparation(Guid orderItemId, int quantity = 1, Guid? actorId = null)
        {
            EnsureSubmittedForKitchenWork();
            GetItem(orderItemId).StartPreparation(quantity, actorId);
            Touch(actorId);
        }

        public void MarkItemReady(Guid orderItemId, int quantity = 1, Guid? actorId = null)
        {
            EnsureSubmittedForKitchenWork();
            GetItem(orderItemId).MarkReady(quantity, actorId);
            Touch(actorId);
        }

        public void ServeItem(Guid orderItemId, int quantity = 1, Guid? actorId = null)
        {
            EnsureSubmittedForKitchenWork();
            GetItem(orderItemId).Serve(quantity, actorId);
            Touch(actorId);
        }

        public void CancelItem(Guid orderItemId, int quantity = 1, Guid? actorId = null)
        {
            if (IsPaid)
                throw new InvalidOperationException(
                    "Paid order cannot be modified");

            if (IsClosed)
                throw new InvalidOperationException(
                    "Closed order cannot be modified");

            GetItem(orderItemId).Cancel(quantity, actorId);
            Touch(actorId);
        }

        public void MarkPaid(Guid? actorId = null)
        {
            if (IsClosed)
                throw new InvalidOperationException("Closed order cannot be paid again");

            if (IsPaid)
                throw new InvalidOperationException("Order already paid");

            if (GetStatus() != OrderStatus.Completed)
                throw new InvalidOperationException("Order can be paid only when all active items are served");

            PaidAt = DateTimeOffset.UtcNow;
            Touch(actorId);
        }

        public void Close(Guid? actorId = null)
        {
            if (!IsPaid)
                throw new InvalidOperationException("Only paid order can be closed");

            if (IsClosed)
                throw new InvalidOperationException("Order already closed");

            ClosedAt = DateTimeOffset.UtcNow;
            Touch(actorId);
        }

        public OrderStatus GetStatus()
        {
            if (IsClosed)
                return OrderStatus.Closed;

            if (IsPaid)
                return OrderStatus.Paid;

            if (!_items.Any())
                return OrderStatus.Draft;

            if (_items.All(i => i.Status == OrderItemStatus.Cancelled))
                return OrderStatus.Cancelled;

            if (!IsSubmitted)
                return OrderStatus.Draft;

            var activeItems = _items.Where(i => i.Status != OrderItemStatus.Cancelled).ToList();

            if (activeItems.Count > 0 && activeItems.All(i => i.Status == OrderItemStatus.Served))
                return OrderStatus.Completed;

            if (activeItems.Any(i => i.Status is OrderItemStatus.InPreparation or OrderItemStatus.Ready))
                return OrderStatus.InProgress;

            return OrderStatus.Submitted;
        }

        public decimal GetTotal()
        {
            return _items.Sum(i => i.GetTotal());
        }

        private OrderItem GetItem(Guid orderItemId)
        {
            return _items.FirstOrDefault(i => i.Id == orderItemId)
                ?? throw new InvalidOperationException("Order item not found");
        }

        private void EnsureSubmittedForKitchenWork()
        {
            if (!IsSubmitted)
                throw new InvalidOperationException("Order must be submitted before kitchen work can start");

            if (IsPaid || IsClosed)
                throw new InvalidOperationException("Paid or closed order cannot be changed by kitchen");
        }

        private void Touch(Guid? actorId)
        {
            LastModifiedBy = actorId;
            LastModifiedAt = DateTimeOffset.UtcNow;
        }
    }
}