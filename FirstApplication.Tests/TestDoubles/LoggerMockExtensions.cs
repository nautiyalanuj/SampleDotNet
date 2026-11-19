using Microsoft.Extensions.Logging;
using Moq;

namespace FirstApplication.Tests.TestDoubles;

internal sealed record LogEntry(LogLevel Level, string Message, Exception? Exception);

internal static class LoggerMockExtensions
{
    /// <summary>
    /// Enables logging on the mock and captures every formatted log entry.
    /// </summary>
    public static List<LogEntry> CaptureLogEntries<T>(this Mock<ILogger<T>> loggerMock)
    {
        ArgumentNullException.ThrowIfNull(loggerMock);

        var entries = new List<LogEntry>();

        loggerMock
            .Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>()))
            .Returns(true);

        loggerMock
            .Setup(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(invocation =>
            {
                var level = (LogLevel)invocation.Arguments[0]!;
                var state = invocation.Arguments[2];
                var exception = (Exception?)invocation.Arguments[3];
                var formatter = (Delegate?)invocation.Arguments[4];

                if (formatter is null)
                {
                    return;
                }

                var message = formatter.DynamicInvoke(state, exception) as string ?? string.Empty;

                entries.Add(new LogEntry(level, message, exception));
            }));

        return entries;
    }
}
