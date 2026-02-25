using System.Globalization;

namespace eWatson.Guards.Resources;

/// <summary>
/// Contains error messages for guard clause validations.
/// </summary>
internal static class GuardMessages
{
    private static readonly System.Resources.ResourceManager Rm =
        new(typeof(GuardMessages));

    public static string ParameterCannotBeNull(string parameterName)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterCannotBeNull", CultureInfo.InvariantCulture)!, parameterName);

    public static string ParameterCannotBeNullOrEmpty(string parameterName)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterCannotBeNullOrEmpty", CultureInfo.InvariantCulture)!, parameterName);

    public static string ParameterCannotBeNullOrWhiteSpace(string parameterName)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterCannotBeNullOrWhiteSpace", CultureInfo.InvariantCulture)!, parameterName);

    public static string ParameterCannotBeNegative(string parameterName, object value)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterCannotBeNegative", CultureInfo.InvariantCulture)!, parameterName, value);

    public static string ParameterMustBeGreaterThanZero(string parameterName, object value)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterMustBeGreaterThanZero", CultureInfo.InvariantCulture)!, parameterName, value);

    public static string ParameterMustBeBetween(string parameterName, object min, object max, object value)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterMustBeBetween", CultureInfo.InvariantCulture)!, parameterName, min, max, value);

    public static string ParameterCannotBeValue(string parameterName, object invalidValue)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterCannotBeValue", CultureInfo.InvariantCulture)!, parameterName, invalidValue);

    public static string ParameterCannotBeEmptyGuid(string parameterName)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterCannotBeEmptyGuid", CultureInfo.InvariantCulture)!, parameterName);

    public static string ParameterExceedsMaxLength(string parameterName, int maxLength, int actualLength)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterExceedsMaxLength", CultureInfo.InvariantCulture)!, parameterName, maxLength, actualLength);

    public static string ParameterBelowMinLength(string parameterName, int minLength, int actualLength)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ParameterBelowMinLength", CultureInfo.InvariantCulture)!, parameterName, minLength, actualLength);
}
