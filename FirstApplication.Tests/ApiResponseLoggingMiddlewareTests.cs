using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FirstApplication.Middleware;
using FirstApplication.Tests.TestDoubles;

namespace FirstApplication.Tests;

[TestClass]
public sealed class ApiResponseLoggingMiddlewareTests
{
    private const string JsonPayload = "{\"temperatureC\":21}";

    private readonly Mock<ILogger<ApiResponseLoggingMiddleware>> _loggerMock = new();

    [TestMethod]
    public async Task InvokeAsync_CopiesResponseBodyToTheClientStream()
    {
        var middleware = new ApiResponseLoggingMiddleware(WriteJsonAsync, _loggerMock.Object);
        var (context, clientStream) = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.AreEqual(JsonPayload, ReadStream(clientStream));
    }

    [TestMethod]
    public async Task InvokeAsync_LogsMethodPathStatusCodeAndResponseBody()
    {
        var entries = _loggerMock.CaptureLogEntries();
        var middleware = new ApiResponseLoggingMiddleware(
            async context =>
            {
                context.Response.StatusCode = StatusCodes.Status201Created;
                await WriteJsonAsync(context);
            },
            _loggerMock.Object);
        var (context, _) = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.HasCount(1, entries);
        var entry = entries[0];
        Assert.AreEqual(LogLevel.Information, entry.Level);
        Assert.Contains("GET", entry.Message);
        Assert.Contains("/WeatherForecast", entry.Message);
        Assert.Contains("201", entry.Message);
        Assert.Contains(JsonPayload, entry.Message);
        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [TestMethod]
    public async Task InvokeAsync_WithBinaryContent_DoesNotLogTheRawBody()
    {
        var entries = _loggerMock.CaptureLogEntries();
        var middleware = new ApiResponseLoggingMiddleware(
            async context =>
            {
                context.Response.ContentType = "application/octet-stream";
                await context.Response.Body.WriteAsync(new byte[] { 1, 2, 3 });
            },
            _loggerMock.Object);
        var (context, _) = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.HasCount(1, entries);
        Assert.DoesNotContain("\u0001\u0002\u0003", entries[0].Message);
        Assert.Contains("bytes of application/octet-stream not logged", entries[0].Message);
    }

    [TestMethod]
    public async Task InvokeAsync_RestoresTheOriginalResponseStream()
    {
        var middleware = new ApiResponseLoggingMiddleware(WriteJsonAsync, _loggerMock.Object);
        var (context, clientStream) = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.AreSame(clientStream, context.Response.Body);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenNextThrows_LogsErrorAndRethrows()
    {
        var entries = _loggerMock.CaptureLogEntries();
        var middleware = new ApiResponseLoggingMiddleware(
            _ => throw new InvalidOperationException("boom"),
            _loggerMock.Object);
        var (context, _) = CreateContext();

        var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => middleware.InvokeAsync(context));

        Assert.AreEqual("boom", exception.Message);
        Assert.HasCount(1, entries);
        Assert.AreEqual(LogLevel.Error, entries[0].Level);
        Assert.Contains("/WeatherForecast", entries[0].Message);
        Assert.AreSame(exception, entries[0].Exception);
    }

    private static (DefaultHttpContext Context, MemoryStream ClientStream) CreateContext()
    {
        var clientStream = new MemoryStream();
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/WeatherForecast";
        context.Response.Body = clientStream;

        return (context, clientStream);
    }

    private static async Task WriteJsonAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonPayload);
    }

    private static string ReadStream(MemoryStream stream) => Encoding.UTF8.GetString(stream.ToArray());
}