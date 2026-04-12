using System;
using System.Collections.Generic;
using System.Text;

namespace DineFlow.Domain
{
    public enum OrderStatus
    {
        Draft,
        Submitted,
        InProgress,
        Completed,
        Paid,
        Closed,
        Cancelled
    }

    public enum OrderItemStatus
    {
        Pending,
        InPreparation,
        Ready,
        Served,
        Cancelled
    }

    public enum OrderType
    {
        DineIn,
        Takeaway
    }
}