namespace eWatson.Abstractions.Specifications.Pagination;

/// <summary>
/// Represents pagination parameters with a 1-based page number and page size.
/// </summary>
/// <param name="Page">The 1-based page number.</param>
/// <param name="Size">The number of items per page.</param>
public sealed record Paging(int Page, int Size)
{
    /// <summary>
    /// Gets a value indicating whether the paging parameters are valid.
    /// Both <see cref="Page"/> and <see cref="Size"/> must be greater than zero.
    /// </summary>
    public bool IsValid => Page > 0 && Size > 0;

    /// <summary>
    /// Gets the number of items to skip for the current page.
    /// Calculated as <c>(Page - 1) * Size</c>.
    /// </summary>
    public int Skip => (Page - 1) * Size;
}
