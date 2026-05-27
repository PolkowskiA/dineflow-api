using System.ComponentModel.DataAnnotations;

namespace DineFlow.Domain.Entities.OrderItems
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = default!;
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public int InPreparationQuantity { get; private set; }
        public int ReadyQuantity { get; private set; }
        public int ServedQuantity { get; private set; }
        public int CancelledQuantity { get; private set; }
        public DateTimeOffset? PreparationStartedAt { get; private set; }
        public DateTimeOffset? ReadyAt { get; private set; }
        public DateTimeOffset? ServedAt { get; private set; }
        public DateTimeOffset? CancelledAt { get; private set; }
        public Guid? LastModifiedBy { get; private set; }
        public DateTimeOffset? LastModifiedAt { get; private set; }

        [Timestamp]
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        public int ActiveQuantity => Quantity - CancelledQuantity;
        public int PendingQuantity => Quantity - AccountedQuantity;

        private int AccountedQuantity => InPreparationQuantity + ReadyQuantity + ServedQuantity + CancelledQuantity;

        public OrderItemStatus Status
        {
            get
            {
                if (CancelledQuantity == Quantity)
                    return OrderItemStatus.Cancelled;

                if (ActiveQuantity > 0 && ServedQuantity == ActiveQuantity)
                    return OrderItemStatus.Served;

                if (ReadyQuantity > 0)
                    return OrderItemStatus.Ready;

                if (InPreparationQuantity > 0)
                    return OrderItemStatus.InPreparation;

                return OrderItemStatus.Pending;
            }
        }

        private OrderItem()
        {
        }

        public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId is required", nameof(productId));

            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("ProductName is required", nameof(productName));

            if (unitPrice < 0)
                throw new ArgumentException("UnitPrice cannot be negative", nameof(unitPrice));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            Id = Guid.NewGuid();
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            ValidateCounters();
        }

        public void StartPreparation(int quantity = 1, Guid? actorId = null)
        {
            EnsurePositiveQuantity(quantity);

            if (PendingQuantity < quantity)
                throw new InvalidOperationException("Not enough pending quantity to start preparation");

            InPreparationQuantity += quantity;
            PreparationStartedAt ??= DateTimeOffset.UtcNow;
            ValidateCounters();
            Touch(actorId);
        }

        public void MarkReady(int quantity = 1, Guid? actorId = null)
        {
            EnsurePositiveQuantity(quantity);

            if (InPreparationQuantity < quantity)
                throw new InvalidOperationException("Not enough in-preparation quantity to mark ready");

            InPreparationQuantity -= quantity;
            ReadyQuantity += quantity;
            ReadyAt ??= DateTimeOffset.UtcNow;
            ValidateCounters();
            Touch(actorId);
        }

        public void Serve(int quantity = 1, Guid? actorId = null)
        {
            EnsurePositiveQuantity(quantity);

            if (ReadyQuantity < quantity)
                throw new InvalidOperationException("Not enough ready quantity to serve");

            ReadyQuantity -= quantity;
            ServedQuantity += quantity;
            ServedAt ??= DateTimeOffset.UtcNow;
            ValidateCounters();
            Touch(actorId);
        }

        public void Cancel(int quantity = 1, Guid? actorId = null)
        {
            EnsurePositiveQuantity(quantity);

            var cancellableQuantity = PendingQuantity + InPreparationQuantity;

            if (cancellableQuantity < quantity)
                throw new InvalidOperationException("Order item can be cancelled only when pending or in preparation");

            var fromPending = Math.Min(PendingQuantity, quantity);
            var remaining = quantity - fromPending;

            if (remaining > 0)
                InPreparationQuantity -= remaining;

            CancelledQuantity += quantity;
            CancelledAt ??= DateTimeOffset.UtcNow;
            ValidateCounters();
            Touch(actorId);
        }

        public decimal GetTotal()
        {
            return ActiveQuantity * UnitPrice;
        }

        private static void EnsurePositiveQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        }

        private void ValidateCounters()
        {
            if (InPreparationQuantity < 0 ||
                ReadyQuantity < 0 ||
                ServedQuantity < 0 ||
                CancelledQuantity < 0)
                throw new InvalidOperationException("Order item counters cannot be negative");

            if (AccountedQuantity > Quantity)
                throw new InvalidOperationException("Order item counters cannot exceed quantity");
        }

        private void Touch(Guid? actorId)
        {
            LastModifiedBy = actorId;
            LastModifiedAt = DateTimeOffset.UtcNow;
        }
    }
}