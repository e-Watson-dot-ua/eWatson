# eWatson

A private .NET Core NuGet package containing foundational domain building blocks for internal projects.

## What's Included

The eWatson package includes all the following components in a single package:

- **Abstractions** - Core interfaces and contracts (entities, value objects, results, events, specifications, pagination)
- **Persistence.Abstractions** - Repository patterns (read/write/combined) and unit of work
- **Guards** - Validation and guard clauses for defensive programming
- **Results** - Result pattern for functional error handling with rich error types
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
- **`IReadRepository<T, TId>`** - Query operations (GetById, Find, Count)
- **`IRepository<T, TId>`** - Combined interface extending both (full CRUD)

### Result Pattern

Functional error handling without exceptions:

```csharp
public Result<Customer> GetCustomer(Guid id)
{
    var customer = _repository.GetById(id);
    return customer is not null
        ? Result.Success(customer)
        : Error.NotFound("Customer not found", "Customer");
}
```

### Specification Pattern

Encapsulate business rules and queries:

```csharp
public class ActiveCustomersSpec : ISpecification<Customer>
{
    public Expression<Func<Customer, bool>> Criteria =>
        c => c.IsActive && !c.IsDeleted;
    // ... other properties
}

var customers = await _repository.FindAsync(new ActiveCustomersSpec());
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
        var order = await _repository.GetByIdAsync(query.OrderId, ct);
        return order is not null
            ? new OrderDto(order)
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
