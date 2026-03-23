using System.Linq.Expressions;

namespace eWatson.Abstractions.Specifications;

/// <summary>
/// Represents an include expression for eager loading related entities
/// of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the root entity.</typeparam>
/// <param name="Expression">The expression defining the navigation
/// property to include.</param>
public sealed record Include<T>(Expression<Func<T, object>> Expression);
