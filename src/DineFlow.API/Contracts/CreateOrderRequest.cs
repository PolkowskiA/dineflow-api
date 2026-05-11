namespace DineFlow.API.Contracts
{
    public class CreateOrderRequest
    {
        public OrderTypeDto? Type { get; set; }
        public Guid? TableId { get; set; }
        public Guid? ActorId { get; set; }
    }
}
