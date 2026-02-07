namespace eWatson.Abstractions.Specifications.Pagination;

/// <summary>
/// Contains pagination metadata for a paged result set.
/// </summary>
/// <param name="Page">Current page number (1-based).</param>
/// <param name="Size">Number of items per page.</param>
/// <param name="TotalItems">Total number of items across all pages.</param>
/// <param name="TotalPages">Total number of pages.</param>
/// <param name="HasNext">Indicates whether a next page exists.</param>
/// <param name="HasPrevious">Indicates whether a previous page exists.</param>
public sealed record PageInfo(int Page, int Size, int TotalItems, int TotalPages,
    bool HasNext, bool HasPrevious)
{
    private static PageInfo Empty(Paging paging, int totalItems) =>
        new(paging.Page, paging.Size, totalItems, 0, false, paging.Page > 1);

    /// <summary>
    /// Creates a <see cref="PageInfo"/> from paging parameters and total item count.
    /// </summary>
    public static PageInfo From(Paging paging, int totalItems)
    {
        if (!paging.IsValid) return Empty(paging, totalItems);
        if (totalItems <= 0) return Empty(paging, 0);

        var totalPages = (int)Math.Ceiling((double)totalItems / paging.Size);

        return new PageInfo(paging.Page, paging.Size, totalItems, totalPages,
            paging.Page < totalPages, paging.Page > 1);
    }
}
