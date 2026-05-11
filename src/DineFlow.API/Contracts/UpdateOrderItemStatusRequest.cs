using DineFlow.Domain;

namespace DineFlow.API.Contracts
{
    public class UpdateOrderItemStatusRequest
    {
        public OrderItemStatus? Status { get; set; }
        public int Quantity { get; set; } = 1;
        public Guid? ActorId { get; set; }
    }
}
