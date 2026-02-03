# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

eWatson is a private .NET Core NuGet package containing foundational domain building blocks for use across internal projects. The package is not published to NuGet.org and is intended for local/private consumption only.

**Core Components:**
- **Domain Entities**: Base classes and interfaces for domain entities
- **Guards**: Validation and assertion logic for defensive programming
- **Primitives**: Value objects and primitive domain types (e.g., Email, PhoneNumber, Money)

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
  - `IEntity`, `IEntity<TId>` - Entity interfaces
  - `IAggregateRoot` - Aggregate root marker
  - `IValueObject` - Value object marker
  - `IDomainEvent` - Domain event interface
  - `IResult` - Result pattern interface (optional)

**eWatson.Entities** (Entity Implementations)
- References: Abstractions
- Contains:
  - `Entity<TId>` - Abstract base class with identity and equality
  - `AggregateRoot` - Base aggregate root with domain events
  - `DomainEvent` - Base domain event class
  - Domain event collection and management

**eWatson.ValueObjects** (Value Object Implementations)
- References: Abstractions, Guards
- Contains:
  - `ValueObject` - Abstract base with structural equality
  - Primitives:
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
  - `Guard.Against.*` - Static guard methods (Null, NullOrEmpty, NegativeOrZero, OutOfRange, InvalidFormat, etc.)
  - `GuardException` - Guard violation exception

**eWatson** (Meta-Package)
- References all projects above
- No source code
- Convenience package for consumers to install everything at once

#### Dependency Graph

```
eWatson.Abstractions (no dependencies)
    ↓
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
├── eWatson.sln                    # Solution file at root
├── Directory.Build.props          # Shared MSBuild properties
├── src/
│   ├── eWatson.Abstractions/
│   │   ├── eWatson.Abstractions.csproj
│   │   ├── Entities/
│   │   │   ├── IEntity.cs
│   │   │   ├── IEntity{TId}.cs
│   │   │   └── IAggregateRoot.cs
│   │   ├── ValueObjects/
│   │   │   └── IValueObject.cs
│   │   └── Events/
│   │       └── IDomainEvent.cs
│   │
│   ├── eWatson.Entities/
│   │   ├── eWatson.Entities.csproj
│   │   ├── Entity.cs
│   │   ├── Entity{TId}.cs
│   │   ├── AggregateRoot.cs
│   │   └── DomainEvents/
│   │       ├── DomainEvent.cs
│   │       └── DomainEventCollection.cs
│   │
│   ├── eWatson.ValueObjects/
│   │   ├── eWatson.ValueObjects.csproj
│   │   ├── ValueObject.cs
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
│   │   └── Exceptions/
│   │       └── GuardException.cs
│   │
│   └── eWatson/
│       └── eWatson.csproj         # Meta-package
│
└── tests/
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
- `eWatson.sln` - Solution file at repository root
- `.gitignore` - .NET specific gitignore
- `README.md` - Package documentation
- `Directory.Build.props` - Shared MSBuild properties (target framework, nullable reference types, versioning)
- `Directory.Build.targets` (optional) - Shared MSBuild targets
- `Directory.Packages.props` (optional) - Central Package Management for NuGet dependencies

**Project directories (src/ProjectName/):**
- `ProjectName.csproj` - Individual project files
- Source code organized by feature/concept

## Key Patterns and Conventions

- **Immutability**: Value objects and primitives should be immutable
- **Validation**: All primitives validate on construction, failing fast with descriptive exceptions
- **No Dependencies**: Keep this package dependency-free (except .NET Core) to avoid version conflicts in consuming projects
- **Explicit Over Implicit**: Favor clarity and explicit naming over brevity
- **Null Safety**: Use nullable reference types consistently (enable `<Nullable>enable</Nullable>`)
- **Testing**: All guards and primitives must have comprehensive unit tests covering edge cases
