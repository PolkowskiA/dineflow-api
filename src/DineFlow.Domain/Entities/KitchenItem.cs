namespace DineFlow.Domain.Entities
{
    public class KitchenItem
    {
        public Guid Id { get; private set; }
        public OrderItemStatus Status { get; private set; }

        public KitchenItem()
        {
            Id = Guid.NewGuid();
            Status = OrderItemStatus.Pending;
        }

        public void StartPreparation()
        {
            EnsureStatus(OrderItemStatus.Pending);

            Status = OrderItemStatus.InPreparation;
        }

        public void MarkReady()
        {
            EnsureStatus(OrderItemStatus.InPreparation);

            Status = OrderItemStatus.Ready;
        }

        public void Serve()
        {
            EnsureStatus(OrderItemStatus.Ready);

            Status = OrderItemStatus.Served;
        }

        public void Cancel(bool allowed)
        {
            if (!allowed)
                throw new InvalidOperationException("Cancellation not allowed");

            if (Status is not OrderItemStatus.Pending and not OrderItemStatus.InPreparation)
                throw new InvalidOperationException("Invalid state for cancellation");

            Status = OrderItemStatus.Cancelled;
        }

        private void EnsureStatus(OrderItemStatus expectedStatus)
        {
            if (Status != expectedStatus)
                throw new InvalidOperationException("Invalid transition");
        }
    }
}
