using System;
using System.Collections.Generic;
using System.Text;

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
            if (Status != OrderItemStatus.Pending)
                throw new InvalidOperationException("Invalid transition");

            Status = OrderItemStatus.InPreparation;
        }

        public void MarkReady()
        {
            if (Status != OrderItemStatus.InPreparation)
                throw new InvalidOperationException("Invalid transition");

            Status = OrderItemStatus.Ready;
        }

        public void Serve()
        {
            if (Status != OrderItemStatus.Ready)
                throw new InvalidOperationException("Invalid transition");

            Status = OrderItemStatus.Served;
        }

        public void Cancel(bool allowed)
        {
            if (!allowed)
                throw new InvalidOperationException("Cancellation not allowed");

            if (Status != OrderItemStatus.Pending && Status != OrderItemStatus.InPreparation)
                throw new InvalidOperationException("Invalid state for cancellation");

            Status = OrderItemStatus.Cancelled;
        }
    }
}