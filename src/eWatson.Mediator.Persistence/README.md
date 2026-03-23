# eWatson.Mediator.Persistence

Provides a mediator pipeline behaviour that wraps command execution in a database transaction using the Unit of Work pattern.

## What it provides

| Type | Description |
|---|---|
| `UnitOfWorkBehavior<TRequest,TResponse>` | Pipeline behaviour — begins, commits, or rolls back a transaction around each command |
| `AddEWatsonMediatorUnitOfWork()` | `IServiceCollection` extension to register the behaviour |

## When to use it

Use this package when:
- You use `eWatson.Mediator` to dispatch commands
- Your infrastructure layer implements `IUnitOfWork` (e.g. wrapping `DbContext`)
- You want automatic transaction management — begin before the handler runs, commit on success, roll back on failure or exception

You do **not** need this package for query-only pipelines. `UnitOfWorkBehavior` is a no-op for any `TResponse` that does not implement `IResult`.

## Pipeline position

Register **after** `AddEWatsonMediator` so the UoW behaviour executes closest to the handler:

```
ExceptionHandling → Logging → Validation → UnitOfWork → Handler
```

```csharp
services.AddEWatsonMediator(opts => { ... })
        .AddEWatsonMediatorUnitOfWork();
```

## Prerequisites

Register your own `IUnitOfWork` implementation before calling `AddEWatsonMediatorUnitOfWork`:

```csharp
services.AddScoped<IUnitOfWork, AppDbContext>();
services.AddEWatsonMediator(...)
        .AddEWatsonMediatorUnitOfWork();
```

## Transaction lifecycle

| Outcome | What the behaviour does |
|---|---|
| Handler succeeds (`IResult.IsSuccess == true`) | `SaveChangesAsync` then `CommitTransactionAsync` |
| Handler returns failure (`IResult.IsSuccess == false`) | `RollbackTransactionAsync` |
| Handler throws an exception | `RollbackTransactionAsync`, then rethrows |
