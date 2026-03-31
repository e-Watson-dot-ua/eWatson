using System.Linq.Expressions;
using eWatson.Abstractions.Specifications.Pagination;

namespace eWatson.Abstractions.Specifications;

/// <summary>
/// Base class for specifications providing fluent API for building queries.
/// Implements <see cref="ICompositeSpecification{T}"/> for combining specifications.
/// </summary>
/// <typeparam name="T">The type being evaluated.</typeparam>
public abstract class Specification<T> : ICompositeSpecification<T>
{
    private readonly List<Include<T>> _includes = [];
    private readonly List<string> _includeStrings = [];
    private readonly List<OrderBy<T>> _orderings = [];

    /// <inheritdoc />
    public Expression<Func<T, bool>> Criteria { get; private set; } = _ => true;

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
    /// Sets the filter criteria for the specification.
    /// Calling this multiple times replaces the previous criteria.
    /// Use <see cref="CombineWith"/> or <see cref="CombineWithOr"/> to compose.
    /// </summary>
    /// <param name="criteria">The filter expression.</param>
    protected void Where(Expression<Func<T, bool>> criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        Criteria = criteria;
    }

    /// <summary>
    /// Adds a navigation property to eager load.
    /// </summary>
    /// <param name="includeExpression">The navigation property expression.</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        ArgumentNullException.ThrowIfNull(includeExpression);
        _includes.Add(new Include<T>(includeExpression));
    }

    /// <summary>
    /// Adds a string-based navigation property path to eager load.
    /// Supports multi-level paths (e.g. <c>"Order.Items.Product"</c>).
    /// </summary>
    /// <param name="includeString">The navigation property path as string.</param>
    protected void AddInclude(string includeString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(includeString);
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
        ArgumentNullException.ThrowIfNull(orderByExpression);
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

    /// <inheritdoc />
    public ICompositeSpecification<T> CombineWith(ISpecification<T> other)
    {
        var combined = CombineCriteria(Criteria, other.Criteria, Expression.AndAlso);
        return new InlineSpecification<T>(combined);
    }

    /// <inheritdoc />
    public ICompositeSpecification<T> CombineWithOr(ISpecification<T> other)
    {
        var combined = CombineCriteria(Criteria, other.Criteria, Expression.OrElse);
        return new InlineSpecification<T>(combined);
    }

    /// <inheritdoc />
    public ICompositeSpecification<T> Invert()
    {
        var param = Criteria.Parameters[0];
        var negated = Expression.Lambda<Func<T, bool>>(
            Expression.Not(Criteria.Body), param);
        return new InlineSpecification<T>(negated);
    }

    /// <summary>
    /// Implicitly converts the specification to its criteria expression.
    /// Returns <c>null</c> when the specification itself is <c>null</c>.
    /// </summary>
    public static implicit operator Expression<Func<T, bool>>?(Specification<T>? specification)
    {
        return specification?.Criteria;
    }

    private static Expression<Func<T, bool>> CombineCriteria(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, BinaryExpression> combiner)
    {
        var param = left.Parameters[0];
        var rightBody = new ParameterReplacer(right.Parameters[0], param)
            .Visit(right.Body);
        var body = combiner(left.Body, rightBody);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    private sealed class ParameterReplacer(
        ParameterExpression oldParam, ParameterExpression newParam)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => node == oldParam ? newParam : base.VisitParameter(node);
    }
}

/// <summary>
/// Internal specification created by composition operations.
/// </summary>
internal sealed class InlineSpecification<T> : Specification<T>
{
    internal InlineSpecification(Expression<Func<T, bool>> criteria) => Where(criteria);
}
