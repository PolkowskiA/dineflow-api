using DineFlow.API.Contracts;
using DineFlow.Application.Orders.Commands.AddItemToOrder;
using DineFlow.Application.Orders.Commands.CreateOrder;
using DineFlow.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DineFlow.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> Create(
            [FromBody] CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            var type = request.Type.ToLower() switch
            {
                "dinein" => OrderType.DineIn,
                "takeaway" => OrderType.Takeaway,
                _ => throw new ArgumentException("Invalid order type")
            };

            var command = new CreateOrderCommand(type, request.TableId);

            var id = await _mediator.Send(command, cancellationToken);

            return Ok(new CreateOrderResponse
            {
                Id = id
            });
        }

        [HttpPost("{orderId}/items")]
        public async Task<IActionResult> AddItem(
            Guid orderId,
            [FromBody] AddItemRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AddItemToOrderCommand(
                orderId,
                request.ProductId,
                request.Quantity
            );

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}