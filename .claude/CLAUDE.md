# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

eWatson is a private .NET Core NuGet package containing foundational domain building blocks for use across internal projects. The package is not published to NuGet.org and is intended for local/private consumption only.

**Core Components:**
- **Domain Entities**: Base classes and interfaces for domain entities
- **Guards**: Validation and assertion logic for defensive programming
- **Primitives**: Value objects and primitive domain types (e.g., Email, PhoneNumber, Money)
- **Results**: Functional error handling with Result/Error pattern

## Development Commands

### Build and Restore
```bash
dotnet restore                    # Restore NuGet dependencies
dotnet build                      # Build the solution
dotnet build --configuration Release  # Build for release
```

### Testing
```bash
dotnet test                       # Run all tests
dotnet test --filter "FullyQualifiedName~Guards"  # Run specific test class
dotnet test --logger "console;verbosity=detailed"  # Detailed test output
```

### Packaging
```bash
dotnet pack                       # Create NuGet package (outputs to bin/Debug or bin/Release)
dotnet pack --configuration Release  # Create release package
dotnet pack -o ./nupkgs          # Output package to specific directory
```

### Local Package Installation
After packing, reference the package in consuming projects by adding the local package source to NuGet.config or install directly:
```bash
dotnet add package eWatson --source ./path/to/nupkgs
```

## Architecture

### Multi-Project Solution Structure

This solution uses a modular multi-project approach for better separation of concerns and granular dependency management.

#### Project Breakdown

**eWatson.Abstractions** (Core Interfaces & Contracts)
- Pure interfaces and base contracts
- No implementations, no external dependencies
- Contains:
  - **Entities/**
    - `IEntity` - Non-generic entity marker
    - `IEntity<TId>` - Entity with strongly-typed identifier
    - `IAggregateRoot` - Aggregate root marker
    - `IAggregateRoot<TId>` - Aggregate root with strongly-typed identifier
    - `IHasDomainEvents` - Domain events capability
  - **Events/**
    - `IDomainEvent` - Domain event interface
    - `IAggregateEvent<TId>` - Event with aggregate identifier
    - `IDomainEventHandler<TEvent>` - Event handler interface
    - `IDomainEventDispatcher` - Event dispatcher interface
  - **ValueObjects/**
    - `IValueObject` - Value object marker
  - **Results/**
    - `IResult` - Operation result without return value
    - `IResult<T>` - Operation result with return value
  - **Auditing/**
    - `IAuditable` - Creation and modification tracking
    - `ISoftDeletable` - Soft deletion support
    - `IHasConcurrencyToken` - Optimistic concurrency control
  - **Specifications/**
    - `ISpecification<T>` - Specification pattern for business rules
    - `ICompositeSpecification<T>` - Composable specifications

**eWatson.Persistence.Abstractions** (Infrastructure Contracts)
- References: Abstractions
- Optional package for persistence patterns
- Contains:
  - **Repositories/**
    - `IRepository<T>` - Base repository for aggregate roots
    - `IRepository<T, TId>` - Repository with strongly-typed identifier
    - `IReadOnlyRepository<T, TId>` - Read-only repository for CQRS queries
  - **UnitOfWork/**
    - `IUnitOfWork` - Transaction management

**eWatson.Results** (Result Pattern Implementations)
- References: Abstractions
- Contains:
  - `Result` - Implements IResult for operations without return values
  - `Result<T>` - Implements IResult<T> for operations with return values
  - `Error` - Represents error information with code, message, and metadata
  - `ResultExtensions` - Functional extensions (Match, Map, Bind, Tap, etc.)
  - **Resources/**
    - `ResultMessages.cs` - Strongly-typed access to error messages
    - `ResultMessages.resx` - Resource file for error messages (localization-ready)

**eWatson.Entities** (Entity Implementations)
- References: Abstractions
- Contains:
  - `Entity<TId>` - Abstract base class with identity-based equality
  - `AggregateRoot<TId>` - Base aggregate root with domain event management
  - **Events/**
    - `DomainEvent` - Base domain event record (immutable)

**eWatson.ValueObjects** (Value Object Implementations)
- References: Abstractions, Guards
- Contains:
  - `ValueObject` - Abstract base with structural equality
  - **Resources/**
    - `ValueObjectMessages.cs` - Strongly-typed access to error messages
    - `ValueObjectMessages.resx` - Resource file for error messages (localization-ready)
  - **Primitives/**
    - `EmailAddress` - Email with validation
    - `PhoneNumber` - Phone number with format support
    - `Money` - Money with currency
    - `Percentage` - Percentage (0-100)
    - `Url` - URL validation
    - `PostalCode` - Postal code
    - `DateRange` - Date range value object


**eWatson.Guards** (Validation & Guard Clauses)
- No dependencies (completely standalone)
- Contains:
  - `Guard.cs` - Static entry point for guard clauses
  - `Guard.Against.cs` - Static class with guard methods (Null, NullOrEmpty, NegativeOrZero, OutOfRange, etc.)
  - `Resources/GuardMessages.cs` - Strongly-typed access to error messages
  - `Resources/GuardMessages.resx` - Resource file for error messages (localization-ready)
  - `Exceptions/GuardException.cs` - Guard violation exception

**Folder Structure:**
```
eWatson.Guards/
├── eWatson.Guards.csproj
├── Guard.cs
├── Guard.Against.cs
├── Resources/
│   ├── GuardMessages.cs
│   └── GuardMessages.resx
└── Exceptions/
    └── GuardException.cs
```

**eWatson** (Meta-Package)
- References all projects above
- No source code
- Convenience package for consumers to install everything at once

#### Dependency Graph

```
eWatson.Abstractions (no dependencies)
    ↓
    ├── eWatson.Persistence.Abstractions (opt-in)
    ├── eWatson.Results → functional error handling
    ├── eWatson.Entities
    ├── eWatson.ValueObjects → uses Guards for validation
    └── eWatson.Guards (standalone, no dependencies)
            ↓
        eWatson (meta-package, references all)
```

### Folder Structure

```
eWatson/
├── .claude/
│   └── CLAUDE.md
├── .gitignore
├── README.md
├── eWatson.slnx                   # Solution file at root
├── Directory.Build.props          # Shared MSBuild properties
├── Directory.Packages.props       # Central Package Management
├── src/
│   ├── eWatson.Abstractions/
│   │   ├── eWatson.Abstractions.csproj
│   │   ├── Entities/
│   │   │   ├── IEntity.cs
│   │   │   ├── IEntity{TId}.cs
│   │   │   ├── IAggregateRoot.cs
│   │   │   ├── IAggregateRoot{TId}.cs
│   │   │   └── IHasDomainEvents.cs
│   │   ├── Events/
│   │   │   ├── IDomainEvent.cs
│   │   │   ├── IAggregateEvent.cs
│   │   │   ├── IDomainEventHandler{T}.cs
│   │   │   └── IDomainEventDispatcher.cs
│   │   ├── ValueObjects/
│   │   │   └── IValueObject.cs
│   │   ├── Results/
│   │   │   ├── IResult.cs
│   │   │   └── IResult{T}.cs
│   │   ├── Auditing/
│   │   │   ├── IAuditable.cs
│   │   │   ├── ISoftDeletable.cs
│   │   │   └── IHasConcurrencyToken.cs
│   │   └── Specifications/
│   │       ├── ISpecification{T}.cs
│   │       └── ICompositeSpecification{T}.cs
│   │
│   ├── eWatson.Persistence.Abstractions/
│   │   ├── eWatson.Persistence.Abstractions.csproj
│   │   ├── Repositories/
│   │   │   ├── IRepository{T}.cs
│   │   │   ├── IRepository{T,TId}.cs
│   │   │   └── IReadOnlyRepository{T,TId}.cs
│   │   └── UnitOfWork/
│   │       └── IUnitOfWork.cs
│   │
│   ├── eWatson.Results/
│   │   ├── eWatson.Results.csproj
│   │   ├── Result.cs
│   │   ├── Result{T}.cs
│   │   ├── Error.cs
│   │   ├── ResultExtensions.cs
│   │   └── Resources/
│   │       ├── ResultMessages.cs
│   │       └── ResultMessages.resx
│   │
│   ├── eWatson.Entities/
│   │   ├── eWatson.Entities.csproj
│   │   ├── Entity{TId}.cs
│   │   ├── AggregateRoot{TId}.cs
│   │   └── Events/
│   │       └── DomainEvent.cs
│   │
│   ├── eWatson.ValueObjects/
│   │   ├── eWatson.ValueObjects.csproj
│   │   ├── ValueObject.cs
│   │   ├── Resources/
│   │   │   ├── ValueObjectMessages.cs
│   │   │   └── ValueObjectMessages.resx
│   │   └── Primitives/
│   │       ├── EmailAddress.cs
│   │       ├── PhoneNumber.cs
│   │       ├── Money.cs
│   │       ├── Percentage.cs
│   │       ├── Url.cs
│   │       ├── PostalCode.cs
│   │       └── DateRange.cs
│   │
│   ├── eWatson.Guards/
│   │   ├── eWatson.Guards.csproj
│   │   ├── Guard.cs
│   │   ├── Guard.Against.cs
│   │   ├── Resources/
│   │   │   ├── GuardMessages.cs
│   │   │   └── GuardMessages.resx
│   │   └── Exceptions/
│   │       └── GuardException.cs
│   │
│   └── eWatson/
│       └── eWatson.csproj         # Meta-package
│
└── tests/                         # [Planned]
    ├── eWatson.Entities.Tests/
    │   ├── eWatson.Entities.Tests.csproj
    │   ├── EntityTests.cs
    │   └── AggregateRootTests.cs
    │
    ├── eWatson.ValueObjects.Tests/
    │   ├── eWatson.ValueObjects.Tests.csproj
    │   ├── ValueObjectTests.cs
    │   └── Primitives/
    │       ├── EmailAddressTests.cs
    │       ├── PhoneNumberTests.cs
    │       └── MoneyTests.cs
    │
    └── eWatson.Guards.Tests/
        ├── eWatson.Guards.Tests.csproj
        └── GuardTests.cs
```

### Setup Files Location

**Root directory:**
- `eWatson.slnx` - Solution file at repository root (XML-based solution format)
- `.gitignore` - .NET specific gitignore
- `README.md` - Package documentation
- `Directory.Build.props` - Shared MSBuild properties (target framework, nullable reference types, versioning)
- `Directory.Build.targets` (optional) - Shared MSBuild targets
- `Directory.Packages.props` - Central Package Management for NuGet dependencies

**Project directories (src/ProjectName/):**
- `ProjectName.csproj` - Individual project files
- Source code organized by feature/concept (namespace folders)

## Key Patterns and Conventions

### General Principles

- **Immutability**: Value objects and primitives should be immutable
- **Validation**: All primitives validate on construction, failing fast with descriptive exceptions
- **No Dependencies**: Keep core packages dependency-free (except .NET) to avoid version conflicts
- **Explicit Over Implicit**: Favor clarity and explicit naming over brevity
- **Null Safety**: Use nullable reference types consistently (enable `<Nullable>enable</Nullable>`)
- **Testing**: All guards and primitives must have comprehensive unit tests covering edge cases
- **Line Length**: Maximum line length is 100 characters. Break longer lines for readability
- **Error Messages**: All user-facing error messages must be stored in .resx resource files with strongly-typed accessor classes (e.g., `GuardMessages.resx` with `GuardMessages.cs`). This enables localization and centralizes message management.

### Naming Conventions

- **CancellationToken Parameters**: Always use `ct` as the parameter name (not `cancellationToken`)
  ```csharp
  // Correct
  Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);

  // Incorrect
  Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
  ```

- **Generic Type Parameters**:
  - `T` - Generic type
  - `TId` - Identifier type
  - `TEvent` - Event type
  - `TKey` - Key type (alternative to TId)

- **Interface Naming**:
  - Marker interfaces: `IEntity`, `IAggregateRoot`, `IValueObject`
  - Capability interfaces: `IHasDomainEvents`, `IAuditable`, `ISoftDeletable`
  - Pattern interfaces: `IRepository<T>`, `ISpecification<T>`, `IResult<T>`

### Architecture Patterns

- **Domain Events**: Only aggregate roots raise domain events via `IHasDomainEvents`
- **Specifications**: Encapsulate business rules using `ISpecification<T>` pattern
- **Result Pattern**: Use `IResult` and `IResult<T>` for functional error handling instead of exceptions for expected failures
- **Repository Pattern**: Repositories work only with aggregate roots (`IAggregateRoot`), not individual entities
- **CQRS Separation**: Use `IRepository<T, TId>` for commands, `IReadOnlyRepository<T, TId>` for queries

### Code Examples

**Entity and Domain Event Usage:**
```csharp
using eWatson.Entities;
using eWatson.Entities.Events;

// Define a domain event
public sealed record OrderPlaced(
    Guid OrderId,
    Guid CustomerId,
    decimal Total
) : DomainEvent;

// Define an aggregate root
public class Order : AggregateRoot<Guid>
{
    public Guid CustomerId { get; private set; }
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }

    public static Order Create(Guid customerId, decimal total)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Total = total,
            Status = OrderStatus.Pending
        };

        order.RaiseDomainEvent(new OrderPlaced(order.Id, customerId, total));
        return order;
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        RaiseDomainEvent(new OrderConfirmed(Id));
    }
}
```

**Error Factory Methods:**
```csharp
using eWatson.Results;

// General error
var error = Error.General("Something went wrong");

// Validation error with optional field name
var validationError = Error.Validation("Invalid email format", "Email");

// Not found error with optional entity name
var notFoundError = Error.NotFound("Customer not found", "Customer");

// Authorization errors
var unauthorizedError = Error.Unauthorized();  // Uses default message
var forbiddenError = Error.Forbidden();        // Uses default message

// Conflict error
var conflictError = Error.Conflict("Email already exists");

// Custom error with metadata
var customError = new Error(
    "Custom.Error",
    "Custom error message",
    new Dictionary<string, object>
    {
        ["Property"] = "Value",
        ["Timestamp"] = DateTime.UtcNow
    }
);
```

**Result Pattern Usage:**
```csharp
using eWatson.Results;

// Basic usage with string errors
public Result<Customer> CreateCustomer(string email)
{
    if (string.IsNullOrEmpty(email))
        return Result.Failure<Customer>("Email is required");

    var customer = new Customer(email);
    return Result.Success(customer);
}

// Using Error class with detailed information
public Result<Order> PlaceOrder(Guid customerId, decimal amount)
{
    if (amount <= 0)
        return Error.Validation("Amount must be greater than zero", "Amount");

    var order = new Order(customerId, amount);
    return order;  // Implicit conversion from T to Result<T>
}

// Functional composition with extensions
public async Task<Result<OrderDto>> GetOrderAsync(Guid orderId)
{
    return await _repository.FindByIdAsync(orderId)
        .ToResult(Error.NotFound($"Order {orderId} not found", "Order"))
        .Map(order => new OrderDto(order))
        .Tap(dto => _logger.LogInformation("Order retrieved: {Id}", dto.Id))
        .TapError(error => _logger.LogWarning("Order not found: {Error}", error));
}

// Pattern matching
var result = CreateCustomer(email);
result.Match(
    onSuccess: customer => Console.WriteLine($"Created: {customer.Name}"),
    onFailure: error => Console.WriteLine($"Failed: {error}")
);

// Chaining operations with Bind
public Result<Invoice> CreateInvoice(Guid orderId)
{
    return GetOrder(orderId)
        .Ensure(order => order.Status == OrderStatus.Completed,
            "Order must be completed")
        .Bind(order => GenerateInvoice(order));
}

// Combining multiple results
var results = new[] { result1, result2, result3 };
Result<IEnumerable<Order>> combined = results.Combine();
// Returns first failure or all values on success
```

**Available Result Extensions:**
- `Match<T, TResult>()` - Pattern match to transform result to another type
- `Match<T>()` - Pattern match to execute actions
- `Map<TIn, TOut>()` - Transform success value
- `Bind<TIn, TOut>()` - Chain result-returning operations (flatMap)
- `Tap<T>()` - Execute side effect on success (e.g., logging)
- `TapError<T>()` - Execute side effect on failure (e.g., logging errors)
- `Ensure<T>()` - Validate success value with predicate
- `ToResult<T>()` - Convert nullable to Result
- `MapAsync<TIn, TOut>()` - Async map transformation
- `BindAsync<TIn, TOut>()` - Async bind operation
- `Combine<T>()` - Combine multiple results into one
```

**Specification Pattern Usage:**
```csharp
public class ActiveCustomerSpec : ISpecification<Customer>
{
    public bool IsSatisfiedBy(Customer customer)
        => customer.IsActive && !customer.IsDeleted;
}

// Usage with repository
var activeCustomers = await _repository.FindAsync(
    new ActiveCustomerSpec(),
    ct);
```
