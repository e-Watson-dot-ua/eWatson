# eWatson

A private .NET Core NuGet package containing foundational domain building blocks for internal projects.

## What's Included

The eWatson package includes all the following components in a single package:

- **Abstractions** - Core interfaces and contracts (entities, value objects, results, events, specifications, pagination)
- **Persistence.Abstractions** - Repository patterns (read/write/combined) and unit of work
- **Primitives** - Result pattern, Maybe pattern, PagedResult, and functional extensions
- **Guards** - Validation and guard clauses for defensive programming
- **Entities** - Entity and aggregate root base classes with domain event support
- **ValueObjects** - Value object base classes and primitives (Email, PhoneNumber, Money, Percentage, URL, PostalCode, DateRange)
- **Mediator** - In-process mediator for CQRS — commands, queries, and notifications with a composable pipeline
- **Mediator.Persistence** - Unit-of-work pipeline behaviour that wraps commands in a database transaction

## Building

```bash
# Windows
.\build-package.ps1

# Linux/Mac
./build-package.sh
```

## Installation

After building, the package is output to `./nupkgs`. Reference it in your projects:

```bash
# Add local package source (one-time setup)
dotnet nuget add source /path/to/eWatson/nupkgs --name eWatson

# Install the package
dotnet add package eWatson
```

Or reference directly:

```bash
dotnet add package eWatson --source /path/to/eWatson/nupkgs
```

## Key Patterns

### Repository Pattern (CQRS-Ready)

Three repository interfaces for flexible data access:

- **`IWriteRepository<T>`** - Command operations (Add, Update, Remove)
- **`IReadRepository<T, TId>`** - Query operations (GetById, Find, Count) — returns `Maybe<T>` for explicit optionality
- **`IRepository<T, TId>`** - Combined interface extending both (full CRUD)

### Result Pattern

Functional error handling without exceptions. Commands and queries always return `Result` or `Result<T>`.

```csharp
public async Task<Result<Order>> GetOrderAsync(Guid id, CancellationToken ct)
{
    Maybe<Order> order = await _repository.GetByIdAsync(id, ct);
    return order.HasValue
        ? order.Value
        : ResultError.NotFound($"Order {id} not found", "Order");
}
```

`ResultError` has static factories for all common error categories:

```csharp
ResultError.General("Something went wrong");
ResultError.Validation("Total must be greater than zero", field: "Total");
ResultError.NotFound("Order not found", entityName: "Order");
ResultError.Unauthorized();
ResultError.Forbidden();
ResultError.Conflict("Order already exists");
ResultError.Internal();
```

`Result<T>` supports implicit conversions — return a value or a `ResultError` directly:

```csharp
// Both are valid return statements from a Task<Result<Order>> method:
return order;                                  // implicit success
return ResultError.NotFound("Not found");      // implicit failure
```

Functional extensions (`Match`, `Map`, `Bind`, `Tap`, `TapError`, `Ensure`, `Combine`, async variants):

```csharp
Result<OrderDto> dto = result
    .Ensure(o => o.IsActive, "Order is inactive")
    .Map(o => new OrderDto(o));

string response = dto.Match(
    onSuccess: d => $"Order {d.Id}",
    onFailure: msg => $"Error: {msg}");
```

### Maybe Pattern

`Maybe<T>` is a readonly struct for explicit optionality — use instead of nullable references.

Use the non-generic `Maybe` class for factory methods:

```csharp
Maybe<Customer> customer = Maybe.Some(existing);
Maybe<Customer> empty    = Maybe.None<Customer>();

// From nullable
Maybe<Customer> maybe = nullableCustomer.ToMaybe();
```

Functional extensions (`Match`, `Map`, `Bind`, `Where`, `OrElse`, `Tap`, `ToResult`):

```csharp
string name = maybe.Match(
    onSome: c => c.Name,
    onNone: () => "Unknown");

Maybe<string> email = maybe
    .Where(c => c.IsActive)
    .Map(c => c.Email);

Result<Customer> result = maybe.ToResult(ResultError.NotFound("Customer not found", "Customer"));
```

### PagedResult

`PagedResult<T>` carries a page of items together with `PageInfo` (page number, page size, total items, total pages).

```csharp
// Create
var paged = PagedResult<OrderDto>.Create(items, paging, totalCount);

// Empty page
var empty = PagedResult<OrderDto>.Empty(paging);

// Project items
PagedResult<OrderSummary> summaries = paged.Map(dto => new OrderSummary(dto));
```

### Specification Pattern

Encapsulate business rules and queries:

```csharp
public class ActiveCustomersSpec : Specification<Customer>
{
    public ActiveCustomersSpec()
    {
        Where(c => c.IsActive && !c.IsDeleted);
    }
}

var customers = await _repository.FindAsync(new ActiveCustomersSpec(), ct);
```

### Mediator Pattern (CQRS)

In-process mediator for dispatching commands, queries, and notifications through a composable pipeline.

#### Registration

```csharp
services.AddEWatsonMediator(
    configure: o =>
    {
        o.EnableValidationBehavior = true;   // off by default
        o.EnableLoggingBehavior    = true;   // on by default
        o.EnableExceptionHandlingBehavior = true; // on by default
        o.PublishStrategy = NotificationPublishStrategy.Sequential;
    },
    typeof(Program).Assembly);

// Optional: wrap commands in a DB transaction (requires IUnitOfWork in DI)
services.AddEWatsonMediatorUnitOfWork();
```

#### Commands

Commands express intent to mutate state and always return `Result` or `Result<T>`.

```csharp
// Define
public sealed record CreateOrderCommand(Guid CustomerId, decimal Total) : ICommand<Guid>;

// Handle
public sealed class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateOrderCommand command, CancellationToken ct)
    {
        var order = Order.Create(command.CustomerId, command.Total);
        await _repository.AddAsync(order, ct);
        return order.Id;
    }
}

// Dispatch
Result<Guid> result = await _mediator.SendAsync(new CreateOrderCommand(customerId, total), ct);
```

#### Queries

Queries are read-only and always return `Result<T>`.

```csharp
// Define
public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderDto>;

// Handle
public sealed class GetOrderHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    public async Task<Result<OrderDto>> HandleAsync(GetOrderQuery query, CancellationToken ct)
    {
        Maybe<Order> order = await _repository.GetByIdAsync(query.OrderId, ct);
        return order.HasValue
            ? new OrderDto(order.Value)
            : ResultError.NotFound($"Order {query.OrderId} not found", "Order");
    }
}

// Dispatch
Result<OrderDto> result = await _mediator.SendAsync(new GetOrderQuery(orderId), ct);
```

#### Notifications

Notifications fan-out to zero or more handlers. Publish strategy is configured via `MediatorOptions.PublishStrategy`.

```csharp
// Define
public sealed record OrderPlacedNotification(Guid OrderId) : INotification;

// Handle (multiple handlers allowed)
public sealed class SendConfirmationEmail : INotificationHandler<OrderPlacedNotification>
{
    public async Task HandleAsync(OrderPlacedNotification n, CancellationToken ct) { /* ... */ }
}

// Publish
await _mediator.PublishAsync(new OrderPlacedNotification(orderId), ct);
```

#### Validation

Implement `IRequestValidator<TRequest>` and enable `EnableValidationBehavior`. All validators for a request are resolved automatically and run before the handler.

```csharp
public sealed class CreateOrderValidator : IRequestValidator<CreateOrderCommand>
{
    public IReadOnlyList<ResultError> Validate(CreateOrderCommand command)
    {
        var errors = new List<ResultError>();
        if (command.Total <= 0)
            errors.Add(ResultError.Validation("Total must be greater than zero", "Total"));
        return errors;
    }
}
```

#### Pipeline Execution Order

```
ExceptionHandling → Logging → Validation → UnitOfWork → Handler
```

Each layer is opt-in/opt-out via `MediatorOptions` at registration time.

## Documentation

For detailed architecture, conventions, and examples, see [.claude/CLAUDE.md](.claude/CLAUDE.md).
