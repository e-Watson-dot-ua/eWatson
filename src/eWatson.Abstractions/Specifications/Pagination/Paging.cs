namespace eWatson.Abstractions.Specifications.Pagination;

/// <summary>
/// Represents pagination parameters with a 1-based page number and page size.
/// </summary>
public sealed record Paging
{
    /// <summary>
    /// Gets the 1-based page number.
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Paging"/> record.
    /// </summary>
    /// <param name="page">The 1-based page number. Must be greater than zero.</param>
    /// <param name="size">The number of items per page. Must be greater than zero.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="page"/> or <paramref name="size"/> is less than or equal to zero.
    /// </exception>
    public Paging(int page, int size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(page, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(size, 0);

        Page = page;
        Size = size;
    }

    /// <summary>
    /// Gets the number of items to skip for the current page.
    /// Calculated as <c>(Page - 1) * Size</c>.
    /// </summary>
    public int Skip => (Page - 1) * Size;
}
