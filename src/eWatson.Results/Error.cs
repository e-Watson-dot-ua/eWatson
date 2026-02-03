using eWatson.Results.Resources;

namespace eWatson.Results;

/// <summary>
/// Represents an error in the system with a code, message, and optional metadata.
/// </summary>
public sealed record Error
{
    /// <summary>
    /// Gets the error code that uniquely identifies this type of error.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets optional metadata associated with the error.
    /// </summary>
    public IReadOnlyDictionary<string, object>? Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="metadata">Optional metadata.</param>
    public Error(string code, string message,
        IReadOnlyDictionary<string, object>? metadata = null)
    {
        Code = code;
        Message = message;
        Metadata = metadata;
    }

    /// <summary>
    /// Gets a general error instance.
    /// </summary>
    public static Error General(string message) =>
        new("General.Error", message);

    /// <summary>
    /// Gets a validation error instance.
    /// </summary>
    public static Error Validation(string message, string? field = null)
    {
        var metadata = field is not null
            ? new Dictionary<string, object> { ["Field"] = field }
            : null;

        return new Error("Validation.Error", message, metadata);
    }

    /// <summary>
    /// Gets a not found error instance.
    /// </summary>
    public static Error NotFound(string message, string? entityName = null)
    {
        var metadata = entityName is not null
            ? new Dictionary<string, object> { ["EntityName"] = entityName }
            : null;

        return new Error("NotFound.Error", message, metadata);
    }

    /// <summary>
    /// Gets an unauthorized error instance.
    /// </summary>
    public static Error Unauthorized(string? message = null) =>
        new("Authorization.Unauthorized",
            message ?? ResultMessages.UnauthorizedAccess());

    /// <summary>
    /// Gets a forbidden error instance.
    /// </summary>
    public static Error Forbidden(string? message = null) =>
        new("Authorization.Forbidden",
            message ?? ResultMessages.ForbiddenAccess());

    /// <summary>
    /// Gets a conflict error instance.
    /// </summary>
    public static Error Conflict(string message) =>
        new("Conflict.Error", message);

    /// <summary>
    /// Returns the error message.
    /// </summary>
    public override string ToString() => Message;
}
