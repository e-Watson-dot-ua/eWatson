namespace eWatson.Results.Resources;

/// <summary>
/// Contains error messages for result operations.
/// </summary>
internal static class ResultMessages
{
    private static readonly System.Resources.ResourceManager _rm =
        new("eWatson.Results.Resources.ResultMessages",
            typeof(ResultMessages).Assembly);

    public static string SuccessResultCannotHaveError()
        => _rm.GetString("SuccessResultCannotHaveError")!;

    public static string FailureResultMustHaveError()
        => _rm.GetString("FailureResultMustHaveError")!;

    public static string CannotAccessValueOfFailedResult(string error)
        => string.Format(_rm.GetString("CannotAccessValueOfFailedResult")!,
            error);

    public static string UnauthorizedAccess()
        => _rm.GetString("UnauthorizedAccess")!;

    public static string ForbiddenAccess()
        => _rm.GetString("ForbiddenAccess")!;
}
