using DineFlow.API.Contracts;
using DineFlow.Application.Common;
using DineFlow.Application.Orders.Commands.AddItemToOrder;
using DineFlow.Application.Orders.Commands.CloseOrder;
using DineFlow.Application.Orders.Commands.CreateOrder;
using DineFlow.Application.Orders.Commands.PayOrder;
using DineFlow.Application.Orders.Commands.SubmitOrder;
using DineFlow.Application.Orders.Commands.UpdateOrderItemStatus;
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
        private readonly IUserContext _userContext;

        public OrdersController(IMediator mediator, IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> Create(
            [FromBody] CreateOrderRequest? request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest("Request body is required.");

            if (request.Type is null)
                return BadRequest("Order type is required.");

            if (request.Items.Count == 0)
                return BadRequest("Order must contain at least one item.");

            var orderType = MapOrderType(request.Type.Value);
            var validationError = ValidateTable(orderType, request.TableId);
            if (validationError is not null)
                return BadRequest(validationError);

            var command = new CreateOrderCommand(
                orderType,
                [.. request.Items.Select(x => new CreateOrderItemDto(x.ProductId, x.Quantity))],
                ResolveActorId(request.ActorId));

            var id = await _mediator.Send(command, cancellationToken);

            return Ok(new CreateOrderResponse { Id = id });
        }

        [HttpPost("{orderId}/items")]
        public async Task<IActionResult> AddItem(
            Guid orderId,
            [FromBody] AddItemRequest? request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest("Request body is required.");

            var command = new AddItemToOrderCommand(
                orderId,
                request.ProductId,
                request.Quantity,
                ResolveActorId(request.ActorId));

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpPost("{orderId}/submit")]
        public async Task<IActionResult> Submit(
            Guid orderId,
            [FromQuery] Guid? actorId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new SubmitOrderCommand(orderId, ResolveActorId(actorId)), cancellationToken);

            return NoContent();
        }

        [HttpPatch("{orderId}/items/{orderItemId}/status")]
        public async Task<IActionResult> UpdateItemStatus(
            Guid orderId,
            Guid orderItemId,
            [FromBody] UpdateOrderItemStatusRequest? request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest("Request body is required.");

            if (request.Status is null)
                return BadRequest("Order item status is required.");

            await _mediator.Send(
                new UpdateOrderItemStatusCommand(
                    orderId,
                    orderItemId,
                    request.Status.Value,
                    request.Quantity,
                    ResolveActorId(request.ActorId)),
                cancellationToken);

            return NoContent();
        }

        [HttpPost("{orderId}/pay")]
        public async Task<IActionResult> Pay(
            Guid orderId,
            [FromQuery] Guid? actorId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new PayOrderCommand(orderId, ResolveActorId(actorId)), cancellationToken);

            return NoContent();
        }

        [HttpPost("{orderId}/close")]
        public async Task<IActionResult> Close(
            Guid orderId,
            [FromQuery] Guid? actorId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new CloseOrderCommand(orderId, ResolveActorId(actorId)), cancellationToken);

            return NoContent();
        }

        private Guid? ResolveActorId(Guid? requestActorId)
        {
            return requestActorId ?? _userContext.UserId;
        }

        private static OrderType MapOrderType(OrderTypeDto orderType)
        {
            return orderType switch
            {
                OrderTypeDto.DineIn => OrderType.DineIn,
                OrderTypeDto.Takeaway => OrderType.Takeaway,
                _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, "Unsupported order type.")
            };
        }

        private static string? ValidateTable(OrderType type, Guid? tableId)
        {
            if (type == OrderType.DineIn && (tableId is null || tableId == Guid.Empty))
                return "TableId is required for dine-in orders.";

            if (type == OrderType.Takeaway && tableId is not null)
                return "TableId must not be provided for takeaway orders.";

            return null;
        }
    }
}