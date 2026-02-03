namespace eWatson.Guards.Exceptions;

/// <summary>
/// Exception thrown when a guard clause validation fails.
/// </summary>
public class GuardException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuardException"/> class.
    /// </summary>
    public GuardException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GuardException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GuardException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GuardException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GuardException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
