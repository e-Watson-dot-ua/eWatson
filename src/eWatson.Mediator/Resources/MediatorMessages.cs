using System.Globalization;

namespace eWatson.Mediator.Resources;

/// <summary>
/// Contains error and diagnostic messages for the mediator.
/// </summary>
internal static class MediatorMessages
{
    private static readonly System.Resources.ResourceManager Rm =
        new(typeof(MediatorMessages));

    public static string HandlerNotFound(string requestTypeName)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("HandlerNotFound", CultureInfo.InvariantCulture)!, requestTypeName);

    public static string MultipleHandlersFound(string requestTypeName)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("MultipleHandlersFound", CultureInfo.InvariantCulture)!, requestTypeName);

    public static string ValidationFailed(string requestTypeName, int errorCount)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("ValidationFailed", CultureInfo.InvariantCulture)!, requestTypeName, errorCount);

    public static string UnhandledException(string requestTypeName)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("UnhandledException", CultureInfo.InvariantCulture)!, requestTypeName);

    public static string UnsupportedPublishStrategy(object strategy)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("UnsupportedPublishStrategy", CultureInfo.InvariantCulture)!, strategy);

    public static string UnsupportedResponseType(string responseTypeName)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("UnsupportedResponseType", CultureInfo.InvariantCulture)!, responseTypeName);
}
