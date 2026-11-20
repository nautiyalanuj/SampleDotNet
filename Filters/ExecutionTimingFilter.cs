using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FirstApplication.Filters
{
    /// <summary>
    /// Async action filter that measures how long an action takes and reports it
    /// in the log and in an X-Execution-Time-Ms response header.
    /// </summary>
    public class ExecutionTimingFilter : IAsyncActionFilter
    {
        private const string HeaderName = "X-Execution-Time-Ms";

        private readonly ILogger<ExecutionTimingFilter> _logger;

        public ExecutionTimingFilter(ILogger<ExecutionTimingFilter> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(next);

            long startTimestamp = Stopwatch.GetTimestamp();

            // Code before 'next' runs before the action; awaiting it runs the action itself.
            ActionExecutedContext executedContext = await next();

            double elapsedMilliseconds = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

            _logger.LogInformation(
                "Action {ActionName} executed in {ElapsedMilliseconds:F1} ms.",
                context.ActionDescriptor.DisplayName,
                elapsedMilliseconds);

            // Headers can only be set while the response has not started streaming.
            if (!executedContext.HttpContext.Response.HasStarted)
            {
                executedContext.HttpContext.Response.Headers[HeaderName] =
                    elapsedMilliseconds.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}
