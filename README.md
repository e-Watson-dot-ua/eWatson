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

## Documentation

For detailed architecture, conventions, and examples, see [.claude/CLAUDE.md](.claude/CLAUDE.md).
