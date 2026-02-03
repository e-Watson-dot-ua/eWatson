namespace eWatson.Abstractions.Auditing;

/// <summary>
/// Indicates that an entity supports soft deletion.
/// Soft-deleted entities are marked as deleted but not physically removed from storage.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets a value indicating whether the entity has been soft-deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Gets the date and time when the entity was deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who deleted the entity.
    /// </summary>
    string? DeletedBy { get; }
}
