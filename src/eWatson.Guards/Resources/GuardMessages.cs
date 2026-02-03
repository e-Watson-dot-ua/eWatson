namespace eWatson.Guards.Resources;

/// <summary>
/// Contains error messages for guard clause validations.
/// </summary>
internal static class GuardMessages
{
    private static readonly System.Resources.ResourceManager _rm =
        new("eWatson.Guards.Resources.GuardMessages", typeof(GuardMessages).Assembly);

    public static string ParameterCannotBeNull(string parameterName)
        => string.Format(_rm.GetString("ParameterCannotBeNull")!, parameterName);

    public static string ParameterCannotBeNullOrEmpty(string parameterName)
        => string.Format(_rm.GetString("ParameterCannotBeNullOrEmpty")!, parameterName);

    public static string ParameterCannotBeNullOrWhiteSpace(string parameterName)
        => string.Format(_rm.GetString("ParameterCannotBeNullOrWhiteSpace")!, parameterName);

    public static string ParameterCannotBeNegative(string parameterName, object value)
        => string.Format(_rm.GetString("ParameterCannotBeNegative")!, parameterName, value);

    public static string ParameterMustBeGreaterThanZero(string parameterName, object value)
        => string.Format(_rm.GetString("ParameterMustBeGreaterThanZero")!, parameterName, value);

    public static string ParameterMustBeBetween(string parameterName, object min, object max, object value)
        => string.Format(_rm.GetString("ParameterMustBeBetween")!, parameterName, min, max, value);

    public static string ParameterCannotBeValue(string parameterName, object invalidValue)
        => string.Format(_rm.GetString("ParameterCannotBeValue")!, parameterName, invalidValue);

    public static string ParameterCannotBeEmptyGuid(string parameterName)
        => string.Format(_rm.GetString("ParameterCannotBeEmptyGuid")!, parameterName);
}
