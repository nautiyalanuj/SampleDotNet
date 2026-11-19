using System.Diagnostics;
using System.Text;

namespace FirstApplication.Middleware
{
    /// <summary>
    /// Buffers the response so the produced payload can be written to the log
    /// before it is flushed to the client.
    /// </summary>
    public sealed class ApiResponseLoggingMiddleware
    {
        private const int MaxLoggedBodyLength = 4096;

        private readonly RequestDelegate _next;
        private readonly ILogger<ApiResponseLoggingMiddleware> _logger;

        public ApiResponseLoggingMiddleware(RequestDelegate next, ILogger<ApiResponseLoggingMiddleware> logger)
        {
            ArgumentNullException.ThrowIfNull(next);
            ArgumentNullException.ThrowIfNull(logger);

            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var originalBody = context.Response.Body;
            var buffer = new MemoryStream();
            var stopwatch = Stopwatch.StartNew();

            context.Response.Body = buffer;

            try
            {
                await _next(context);
                stopwatch.Stop();

                var body = ReadLoggedBody(buffer, context.Response.ContentType);

                buffer.Position = 0;
                await buffer.CopyToAsync(originalBody);

                _logger.LogInformation(
                    "API response {Method} {Path} returned {StatusCode} in {ElapsedMilliseconds} ms. Body: {ResponseBody}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    body);
            }
            catch (Exception exception)
            {
                stopwatch.Stop();

                _logger.LogError(
                    exception,
                    "API request {Method} {Path} failed after {ElapsedMilliseconds} ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
            finally
            {
                context.Response.Body = originalBody;
                await buffer.DisposeAsync();
            }
        }

        private static string ReadLoggedBody(MemoryStream buffer, string? contentType)
        {
            if (buffer.Length == 0)
            {
                return "<empty>";
            }

            if (!IsTextContent(contentType))
            {
                return $"<{buffer.Length} bytes of {contentType ?? "unknown content"} not logged>";
            }

            var body = Encoding.UTF8.GetString(buffer.ToArray());

            return body.Length <= MaxLoggedBodyLength
                ? body
                : body[..MaxLoggedBodyLength] + "... (truncated)";
        }

        private static bool IsTextContent(string? contentType) =>
            contentType is null
            || contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("json", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("xml", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("javascript", StringComparison.OrdinalIgnoreCase);
    }
}
