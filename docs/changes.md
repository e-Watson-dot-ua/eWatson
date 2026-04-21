# Actionable Tasks

## Completed

- [x] Preserve structured errors across `Result` transformations.
  Deliverables:
  Update `Map`, `Bind`, `MapAsync`, and `BindAsync` in `src/eWatson.Primitives/Results/Result.Extensions.cs` so failure results preserve the original `Errors` collection.
  Add targeted tests in `tests/eWatson.Primitives.Tests/ResultExtensions.Tests.cs` that verify structured error propagation for sync and async flows.

- [x] Prevent invalid failure results with empty error collections.
  Deliverables:
  Add guards in `Result.Failure(IReadOnlyList<ResultError>)` and `Result.Failure<T>(IReadOnlyList<ResultError>)`.
  Add tests in `tests/eWatson.Primitives.Tests/Result.Tests.cs` for `null` and empty collections.

- [x] Align the README with the actual public API.
  Deliverables:
  Replace outdated mediator registration examples with `AddMediator()` and `AddMediatorUnitOfWork()`.
  Re-read the mediator section to ensure naming and examples match the current package surface.

- [x] Expand test coverage for missing value objects.
  Deliverables:
  Add `EmailAddress.Tests.cs`, `DateRange.Tests.cs`, `PostalCode.Tests.cs`, and `Url.Tests.cs` under `tests/eWatson.ValueObjects.Tests`.
  Cover construction, normalization, invalid input, and representative happy-path behavior.

## Next

## Later

- [x] Reduce packaging fragility in the umbrella NuGet project.
  Deliverables:
  Rework `src/eWatson/eWatson.csproj` to avoid maintaining a hardcoded list of package outputs.
  Add a validation step or test that asserts the expected package contents.

- [x] Strengthen domain rules in value objects.
  Deliverables:
  Tighten validation behavior in `EmailAddress` and `Money`.
  Document where the library intentionally uses pragmatic validation rather than strict RFC/ISO compliance.

- [x] Add integration-style tests for mediator and event pipelines.
  Deliverables:
  Cover behavior composition, structured error propagation, listener failures, and cancellation behavior.

- [x] Reduce reflection overhead in `DomainEventDispatcher`.
  Deliverables:
  Replace or cache reflection-based listener invocation in `src/eWatson.Events/DomainEventDispatcher.cs`.
  Add tests around exception and cancellation propagation if the dispatch implementation changes.

- [x] Revisit framework targeting strategy.
  Deliverables:
  Evaluate whether `net8.0` plus `net10.0` multi-targeting is worth the added maintenance cost.
  Document the chosen targeting strategy in the README or release notes.
