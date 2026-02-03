# eWatson

A private .NET Core NuGet package containing foundational domain building blocks for internal projects.

## What's Included

The eWatson package includes all the following components in a single package:

- **Abstractions** - Core interfaces and contracts (entities, value objects, results, events, specifications)
- **Persistence.Abstractions** - Repository and unit of work patterns
- **Guards** - Validation and guard clauses for defensive programming
- **Results** - Result pattern for functional error handling
- **Entities** - Entity and aggregate root base classes with domain event support
- **ValueObjects** - Value object base classes and primitives (Email, PhoneNumber, Money, etc.)

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
