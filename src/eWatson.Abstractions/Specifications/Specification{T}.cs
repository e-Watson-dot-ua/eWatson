using System.Linq.Expressions;
using eWatson.Abstractions.Specifications.Pagination;

namespace eWatson.Abstractions.Specifications;

/// <summary>
/// Base class for specifications providing fluent API for building queries.
/// </summary>
/// <typeparam name="T">The type being evaluated.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    private readonly List<Include<T>> _includes = [];
    private readonly List<string> _includeStrings = [];
    private readonly List<OrderBy<T>> _orderings = [];

    /// <inheritdoc />
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<Include<T>> Includes => _includes.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyList<string> IncludeStrings => _includeStrings.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyList<OrderBy<T>> Orderings => _orderings.AsReadOnly();

    /// <inheritdoc />
    public Paging? Paging { get; private set; }

    /// <inheritdoc />
    public bool AsNoTracking { get; private set; } = true;

    /// <inheritdoc />
    public bool Distinct { get; private set; }

    /// <summary>
    /// Adds a filter criteria to the specification.
    /// </summary>
    /// <param name="criteria">The filter expression.</param>
    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adds a navigation property to eager load.
    /// </summary>
    /// <param name="includeExpression">The navigation property expression.</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        _includes.Add(new Include<T>(includeExpression));
    }

    /// <summary>
    /// Adds a string-based navigation property path to eager load.
    /// Supports multi-level paths (e.g. <c>"Order.Items.Product"</c>).
    /// </summary>
    /// <param name="includeString">The navigation property path as string.</param>
    protected void AddInclude(string includeString)
    {
        _includeStrings.Add(includeString);
    }

    /// <summary>
    /// Adds an ordering expression (ascending by default).
    /// </summary>
    /// <param name="orderByExpression">The property to order by.</param>
    /// <param name="descending">True for descending order.</param>
    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression,
        bool descending = false)
    {
        _orderings.Add(new OrderBy<T>(orderByExpression, descending));
    }

    /// <summary>
    /// Applies pagination to the query.
    /// </summary>
    /// <param name="page">The 1-based page number.</param>
    /// <param name="size">The number of items per page.</param>
    protected void ApplyPaging(int page, int size)
    {
        Paging = new Paging(page, size);
    }

    /// <summary>
    /// Applies pagination to the query.
    /// </summary>
    /// <param name="paging">The pagination parameters.</param>
    protected void ApplyPaging(Paging paging)
    {
        Paging = paging;
    }

    /// <summary>
    /// Enables no-tracking for the query (read-only optimization).
    /// </summary>
    protected void EnableNoTracking()
    {
        AsNoTracking = true;
    }

    /// <summary>
    /// Enables distinct filtering for the query.
    /// </summary>
    protected void EnableDistinct()
    {
        Distinct = true;
    }

    /// <summary>
    /// Implicitly converts the specification to its criteria expression.
    /// </summary>
    /// <param name="specification">The specification to convert.</param>
    /// <returns>The criteria expression, or null if no criteria is defined.</returns>
    public static implicit operator Expression<Func<T, bool>>?(Specification<T>? specification)
    {
        return specification?.Criteria;
    }
}
