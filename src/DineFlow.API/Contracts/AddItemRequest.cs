namespace DineFlow.API.Contracts
{
    public class AddItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}