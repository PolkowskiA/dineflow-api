namespace DineFlow.API.Contracts
{
    public class CreateOrderRequest
    {
        public string Type { get; set; } = default!;
        public Guid? TableId { get; set; }
    }
}