namespace eWatson.Abstractions.Specifications.Pagination;

/// <summary>
/// Represents pagination parameters with a 1-based page number and page size.
/// </summary>
/// <param name="Page">The 1-based page number.</param>
/// <param name="Size">The number of items per page.</param>
public sealed record Paging(int Page, int Size)
{
    public bool IsValid => Page > 0 && Size > 0;
    public int Skip => (Page - 1) * Size;
}
