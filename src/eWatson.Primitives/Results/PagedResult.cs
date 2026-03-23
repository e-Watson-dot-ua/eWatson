using System.Diagnostics.CodeAnalysis;
using eWatson.Abstractions.Specifications.Pagination;

namespace eWatson.Primitives.Results;

/// <summary>
/// Represents a paged result set containing items and pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the result set.</typeparam>
/// <param name="Items">The items for the current page.</param>
/// <param name="PageInfo">The pagination metadata.</param>
[SuppressMessage("Design", "CA1000:Do not declare static members on generic types",
    Justification = "Factory methods require the type parameter for ergonomic API")]
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    PageInfo PageInfo)
{
    /// <summary>
    /// Creates an empty paged result for the given paging parameters.
    /// </summary>
    /// <param name="paging">The paging parameters.</param>
    public static PagedResult<T> Empty(Paging paging) =>
        new([], PageInfo.From(paging, 0));

    /// <summary>
    /// Creates a paged result from items, paging parameters, and total count.
    /// </summary>
    /// <param name="items">The items for the current page.</param>
    /// <param name="paging">The paging parameters.</param>
    /// <param name="totalItems">The total number of items across all pages.</param>
    public static PagedResult<T> Create(
        IReadOnlyList<T> items, Paging paging, int totalItems) =>
        new(items, PageInfo.From(paging, totalItems));

    /// <summary>
    /// Transforms the items using the provided mapping function.
    /// </summary>
    /// <typeparam name="TOut">The output item type.</typeparam>
    /// <param name="mapper">The mapping function.</param>
    public PagedResult<TOut> Map<TOut>(Func<T, TOut> mapper) =>
        new(Items.Select(mapper).ToList(), PageInfo);
}
