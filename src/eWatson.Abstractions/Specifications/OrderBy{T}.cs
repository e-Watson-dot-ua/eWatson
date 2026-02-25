using System.Linq.Expressions;

namespace eWatson.Abstractions.Specifications;

/// <summary>
/// Defines an ordering clause for a specification query.
/// </summary>
/// <typeparam name="T">The entity type to order.</typeparam>
/// <param name="KeySelector">Expression selecting the property to order by</param>
/// <param name="Descending">If true, orders in descending order; otherwise ascending</param>
public sealed record class OrderBy<T>(Expression<Func<T, object>> KeySelector,
    bool Descending = false);
    