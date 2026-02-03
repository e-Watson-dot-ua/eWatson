namespace eWatson.Guards.Resources;

/// <summary>
/// Contains error messages for guard clause validations.
/// </summary>
internal static class GuardMessages
{
    public static string ParameterCannotBeNull(string parameterName)
        => $"Parameter '{parameterName}' cannot be null.";

    public static string ParameterCannotBeNullOrEmpty(string parameterName)
        => $"Parameter '{parameterName}' cannot be null or empty.";

    public static string ParameterCannotBeNullOrWhiteSpace(string parameterName)
        => $"Parameter '{parameterName}' cannot be null, empty, or whitespace.";

    public static string ParameterCannotBeNegative(string parameterName, object value)
        => $"Parameter '{parameterName}' cannot be negative. Value: {value}";

    public static string ParameterMustBeGreaterThanZero(
        string parameterName,
        object value)
        => $"Parameter '{parameterName}' must be greater than zero. Value: {value}";

    public static string ParameterMustBeBetween(
        string parameterName,
        object min,
        object max,
        object value)
        => $"Parameter '{parameterName}' must be between {min} and {max}. "
            + $"Value: {value}";

    public static string ParameterCannotBeValue(
        string parameterName,
        object invalidValue)
        => $"Parameter '{parameterName}' cannot be '{invalidValue}'.";

    public static string ParameterCannotBeEmptyGuid(string parameterName)
        => $"Parameter '{parameterName}' cannot be an empty Guid.";
}
