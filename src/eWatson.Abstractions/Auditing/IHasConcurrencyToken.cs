namespace eWatson.Abstractions.Auditing;

/// <summary>
/// Indicates that an entity has a concurrency token for optimistic concurrency control.
/// </summary>
public interface IHasConcurrencyToken
{
    /// <summary>
    /// Gets the concurrency token used for optimistic concurrency checks.
    /// Typically a row version or timestamp.
    /// </summary>
    byte[]? ConcurrencyToken { get; }
}
