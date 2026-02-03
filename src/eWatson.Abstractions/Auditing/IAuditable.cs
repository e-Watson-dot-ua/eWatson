namespace eWatson.Abstractions.Auditing;

/// <summary>
/// Indicates that an entity tracks creation and modification audit information.
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who created the entity.
    /// </summary>
    string CreatedBy { get; }

    /// <summary>
    /// Gets the date and time when the entity was last modified.
    /// </summary>
    DateTimeOffset? ModifiedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who last modified the entity.
    /// </summary>
    string? ModifiedBy { get; }
}
