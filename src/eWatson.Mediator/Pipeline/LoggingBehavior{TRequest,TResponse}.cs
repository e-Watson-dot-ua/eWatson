using eWatson.Abstractions.Results;
using eWatson.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace eWatson.Mediator.Pipeline;

/// <summary>
/// Pipeline behaviour that logs the request name, elapsed time, and outcome.
/// Logs at <see cref="LogLevel.Information"/> on success and
/// <see cref="LogLevel.Warning"/> on failure.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed partial class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="LoggingBehavior{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="logger">Logger for request lifecycle events.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestContinuation<TResponse> continuation,
        CancellationToken ct = default)
    {
        var requestName = typeof(TRequest).Name;
        LogHandling(_logger, requestName);

        var sw = Stopwatch.StartNew();
        var response = await continuation(ct).ConfigureAwait(false);
        sw.Stop();

        if (response is IResult { IsFailure: true })
            LogFailure(_logger, requestName, sw.ElapsedMilliseconds);
        else
            LogSuccess(_logger, requestName, sw.ElapsedMilliseconds);

        return response;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Handling {RequestName}")]
    private static partial void LogHandling(ILogger logger, string requestName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Request {RequestName} completed in {ElapsedMs}ms")]
    private static partial void LogSuccess(ILogger logger, string requestName, long elapsedMs);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Request {RequestName} failed in {ElapsedMs}ms")]
    private static partial void LogFailure(ILogger logger, string requestName, long elapsedMs);
}
