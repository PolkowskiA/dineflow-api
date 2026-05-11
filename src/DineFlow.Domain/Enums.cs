namespace DineFlow.Domain
{
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
