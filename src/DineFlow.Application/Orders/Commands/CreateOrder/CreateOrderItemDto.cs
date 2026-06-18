namespace DineFlow.Application.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderItemDto(Guid ProductId, int Quantity);
}