using System.Globalization;

namespace eWatson.Results.Resources;

/// <summary>
/// Contains error messages for result operations.
/// </summary>
internal static class ResultMessages
{
    private static readonly System.Resources.ResourceManager Rm =
        new(typeof(ResultMessages));

    public static string SuccessResultCannotHaveError()
        => Rm.GetString("SuccessResultCannotHaveError", CultureInfo.InvariantCulture)!;

    public static string FailureResultMustHaveError()
        => Rm.GetString("FailureResultMustHaveError", CultureInfo.InvariantCulture)!;

    public static string CannotAccessValueOfFailedResult(string error)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("CannotAccessValueOfFailedResult", CultureInfo.InvariantCulture)!, error);

    public static string UnauthorizedAccess()
        => Rm.GetString("UnauthorizedAccess", CultureInfo.InvariantCulture)!;

    public static string ForbiddenAccess()
        => Rm.GetString("ForbiddenAccess", CultureInfo.InvariantCulture)!;

    public static string InternalError()
        => Rm.GetString("InternalError", CultureInfo.InvariantCulture)!;
}
