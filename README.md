# DineFlow API

DineFlow API is a .NET backend for managing a restaurant order lifecycle. The current version focuses on the domain and application flow for creating orders, adding products, submitting orders to the kitchen, progressing item statuses, taking payment, and closing completed orders.

The project currently uses in-memory implementations for orders and products, so it is ready for local development and domain validation, but it does not persist data between application restarts yet.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- MediatR
- Scalar API reference
- xUnit
- FluentAssertions

## Solution Structure

```text
src/
  DineFlow.API/             HTTP API, controllers, request/response contracts
  DineFlow.Application/     Use cases, command handlers, application interfaces
  DineFlow.Domain/          Domain entities, enums, lifecycle rules, domain exceptions
  DineFlow.Infrastructure/  Infrastructure project placeholder for persistence/integrations

tests/
  DineFlow.Tests/           Domain and application tests
```

## Current Capabilities

- Create dine-in and takeaway orders.
- Validate table rules:
  - dine-in orders require a `tableId`
  - takeaway orders must not include a `tableId`
- Add products to a draft order.
- Submit an order once it contains at least one active item.
- Move order items through kitchen states:
  - `pending`
  - `in_preparation`
  - `ready`
  - `served`
  - `cancelled`
- Support partial quantity transitions for order items.
- Calculate totals from active quantities only.
- Pay an order only after all active items are served.
- Close an order only after it is paid.
- Track optional actor IDs for create/modify operations.
- Expose `RowVersion` fields on domain entities for future optimistic concurrency mapping.

## Current Limitations

- Orders are stored in memory through `FakeOrderRepository`.
- Products are served from `FakeProductService`.
- No database provider is configured yet.
- No authentication scheme is configured yet. `ApiUserContext` can read an authenticated user ID from `ClaimTypes.NameIdentifier` or `sub`, but the API does not currently set up authentication.
- There are no query endpoints yet for reading orders back from the API.

## Getting Started

### Prerequisites

- .NET 10 SDK

### Restore and Build

```bash
dotnet restore
dotnet build DineFlow.slnx
```

### Run the API

```bash
dotnet run --project src/DineFlow.API
```

The HTTP launch profile uses:

```text
http://localhost:5272
```

In Development, the API also exposes:

```text
/openapi/v1.json
/scalar/v1
```

### Run Tests

```bash
dotnet test
```

## API Endpoints

All endpoints are under:

```text
/api/orders
```

Enum values are serialized as snake_case strings.

### Create Order

```http
POST /api/orders
Content-Type: application/json
```

Dine-in order:

```json
{
  "type": "dine_in",
  "tableId": "11111111-1111-1111-1111-111111111111",
  "actorId": "22222222-2222-2222-2222-222222222222"
}
```

Takeaway order:

```json
{
  "type": "takeaway",
  "actorId": "22222222-2222-2222-2222-222222222222"
}
```

Response:

```json
{
  "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
}
```

### Add Item

```http
POST /api/orders/{orderId}/items
Content-Type: application/json
```

```json
{
  "productId": "6cfd89e6-0c26-42d1-9c38-f68f4d2f7d6f",
  "quantity": 2,
  "actorId": "22222222-2222-2222-2222-222222222222"
}
```

Available fake products:

| Product | ID | Price |
| --- | --- | ---: |
| Burger | `6cfd89e6-0c26-42d1-9c38-f68f4d2f7d6f` | 10 |
| Fries | `f1b8b2db-f754-4d5f-98f4-2c6d08dd7207` | 5 |

Returns `204 No Content`.

### Submit Order

```http
POST /api/orders/{orderId}/submit?actorId=22222222-2222-2222-2222-222222222222
```

Returns `204 No Content`.

### Update Order Item Status

```http
PATCH /api/orders/{orderId}/items/{orderItemId}/status
Content-Type: application/json
```

```json
{
  "status": "in_preparation",
  "quantity": 1,
  "actorId": "22222222-2222-2222-2222-222222222222"
}
```

Supported target statuses:

- `in_preparation`
- `ready`
- `served`
- `cancelled`

`pending` is not a supported transition target through the API.

Returns `204 No Content`.

### Pay Order

```http
POST /api/orders/{orderId}/pay?actorId=22222222-2222-2222-2222-222222222222
```

An order can be paid only when all active item quantities are served.

Returns `204 No Content`.

### Close Order

```http
POST /api/orders/{orderId}/close?actorId=22222222-2222-2222-2222-222222222222
```

An order can be closed only after it has been paid.

Returns `204 No Content`.

## Order Lifecycle

```text
Draft -> Submitted -> InProgress -> Completed -> Paid -> Closed
```

Additional rules:

- An order without items stays in `Draft`.
- Items cannot be added after submission.
- An order cannot be submitted if it has no active items.
- An order is `InProgress` when at least one active item is in preparation or ready.
- An order is `Completed` when all active quantities are served.
- An order is `Cancelled` when all items are cancelled.
- Cancelled quantities are excluded from the order total.

## Error Handling

The API returns Problem Details responses for known validation and domain errors:

- `400 Bad Request` for invalid requests, invalid state transitions, and domain rule violations.
- `404 Not Found` when an order or product cannot be found.

## Development Notes

This codebase currently emphasizes clean domain behavior and command-based application use cases. The next natural steps are adding persistence in `DineFlow.Infrastructure`, read/query endpoints, authentication, and integration tests for the HTTP API.
