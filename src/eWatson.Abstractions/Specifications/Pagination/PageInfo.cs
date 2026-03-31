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
    /// <summary>
    /// Creates a <see cref="PageInfo"/> from paging parameters and total item count.
    /// </summary>
    public static PageInfo From(Paging paging, int totalItems)
    {
        if (totalItems <= 0)
        {
            return new PageInfo(paging.Page, paging.Size, 0, 0,
                false, false);
        }

        var totalPages = (int)Math.Ceiling((double)totalItems / paging.Size);

        return new PageInfo(paging.Page, paging.Size, totalItems, totalPages,
            paging.Page < totalPages, paging.Page > 1);
    }
}
