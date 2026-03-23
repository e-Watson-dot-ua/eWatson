# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Repo Is

**eWatson** is a private .NET 10 NuGet package of foundational DDD building blocks. All library projects live under `src/` and are bundled into a single `eWatson` NuGet package by `src/eWatson/eWatson.csproj`. Tests live under `tests/`.

## Commands

```bash
# Build & test
dotnet build eWatson.slnx
dotnet test eWatson.slnx

# Run a single test project
dotnet test tests/eWatson.Guards.Tests/eWatson.Guards.Tests.csproj

# Run a single test by name (filter)
dotnet test --filter "FullyQualifiedName~GuardAgainstNullTests"

# Build & pack NuGet (outputs to ./nupkgs)
.\build-package.ps1                        # Release, with symbols
.\build-package.ps1 -Version 1.2.0-preview # specific version
.\build-package.ps1 -Configuration Debug
```

## Architecture

The solution is split into thin, focused library projects that depend on each other in one direction:

```
eWatson.Abstractions          ← core DDD interfaces (entities, specs, events, results, pagination)
eWatson.Persistence.Abstractions  ← IReadRepository<T,TId>, IWriteRepository<T>, IRepository<T,TId>, IUnitOfWork
eWatson.Primitives            ← Result, Maybe, PagedResult, ResultError, functional extensions
eWatson.Guards                ← Guard.Against.* static guard clauses (throws GuardException)
eWatson.Entities              ← Entity<TId>, AggregateRoot<TId>, DomainEvent base types
eWatson.ValueObjects          ← ValueObject base + primitives: Email, PhoneNumber, Money, Percentage, Url, PostalCode, DateRange
eWatson.Mediator.Abstractions ← IMediator, ICommand/IQuery/INotification marker interfaces, MediatorOptions, IPipelineBehavior
eWatson.Mediator              ← Mediator implementation, DI registration (AddEWatsonMediator), pipeline wiring
eWatson.Mediator.Persistence  ← UnitOfWorkBehavior<TRequest,TResponse> + AddEWatsonMediatorUnitOfWork()
src/eWatson                   ← umbrella project that re-exports all of the above as one NuGet package
```

### File naming conventions

- Extension classes use dot notation: `Maybe.Extensions.cs`, `Result.Extensions.cs`
- Test classes use dot notation: `Result.Tests.cs`, `GuardAgainstNull.Tests.cs`
- Generic types use braces: `Maybe{T}.cs`, `Result{T}.cs`, `Entity{TId}.cs`

### Key design decisions

**Result pattern** — `Result` / `Result<T>` are the only allowed return types from commands and queries. Never throw for business-logic failures; use `Result.Failure(ResultError.NotFound(...))` etc. `ResultError` has static factories: `General`, `Validation`, `NotFound`, `Unauthorized`, `Forbidden`, `Conflict`, `Internal`. `ResultExtensions` provides functional helpers (`Match`, `Map`, `Bind`, `Tap`, `Ensure`, `Combine`, async variants). Files live in `src/eWatson.Primitives/Result/`.

**Maybe pattern** — `Maybe<T>` is a readonly struct for explicit optionality. Use the non-generic `Maybe` class for factory methods: `Maybe.Some(value)`, `Maybe.None<T>()`. Never use `Maybe<T>.Some` or `Maybe<T>.None` directly — the non-generic `Maybe` class avoids CA1000 and provides cleaner API. Files live in `src/eWatson.Primitives/Maybes/`.

**Persistence abstractions** — `IReadRepository<T,TId>.GetByIdAsync` and `FindFirstAsync` return `Task<Maybe<T>>` instead of `Task<T?>`. This makes optionality explicit and composable with the Maybe extensions.

**Guard clauses** — `Guard` is a static partial class split across `Guard.cs` (entry point) and `Guard.Against.cs` (all methods). All guards throw `GuardException`. Use `[CallerArgumentExpression]` — parameter name is inferred automatically. New guard methods belong in `Guard.Against.cs` or in a new `Guard.*.cs` partial.

**Specification pattern** — Inherit from `Specification<T>` and call the protected builder methods in the constructor: `Where`, `AddInclude`, `AddOrderBy`, `ApplyPaging`, `EnableDistinct`. `AsNoTracking` defaults to `true`. Supports composition via `CombineWith`, `CombineWithOr`, and `Invert`.

**Mediator pipeline** — Fixed execution order (outermost → innermost):
```
ExceptionHandling → Logging → Validation → UnitOfWork → Handler
```
`ExceptionHandling` and `Logging` are on by default; `Validation` is opt-in (`EnableValidationBehavior = true`). `UnitOfWork` is registered separately via `AddEWatsonMediatorUnitOfWork()`. Validation is done by implementing `IRequestValidator<TRequest>` — all validators for a given request type are resolved from DI and run before the handler.

**Command vs Query distinction** — Commands implement `ICommand` (void) or `ICommand<TValue>` (returns `Result<TValue>`). Queries implement `IQuery<TResponse>` (returns `Result<TResponse>`). Both are subtypes of `IRequest<TResponse>`. The `UnitOfWorkBehavior` is a no-op unless `TResponse` implements `IResult`.

**Domain events** — Aggregate roots inherit `AggregateRoot<TId>` and call `RaiseDomainEvent(new MyEvent(...))` inside domain methods. Infrastructure is responsible for dispatching via `IDomainEventDispatcher` and calling `ClearDomainEvents()` after dispatch.

**String resources** — All user-facing messages are kept in `Resources/*.cs` files per project (e.g. `GuardMessages`, `ResultMessages`, `MediatorMessages`, `ValueObjectMessages`). Do not inline message strings.

## Testing

Tests use **xUnit**, **FluentAssertions**, and **NSubstitute** (mocking). Each library project has a corresponding `tests/eWatson.<Name>.Tests` project. Test class naming mirrors the subject: `GuardAgainstNullTests`, `MediatorTests`, `PipelineBehaviorTests`, etc.
