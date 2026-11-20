using FirstApplication.Attributes;
using FirstApplication.Examples;
using FirstApplication.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter<ExecutionTimingFilter>]
    public class AsyncExampleController : ControllerBase
    {
        private const int MaxDelayMilliseconds = 5000;

        private readonly ILogger<AsyncExampleController> _logger;

        public AsyncExampleController(ILogger<AsyncExampleController> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }

        /// <summary>
        /// Awaits a single simulated I/O-bound operation without blocking the request thread.
        /// </summary>
        [HttpGet("delay")]
        public async Task<ActionResult<string>> GetDelayedAsync(
            [FromQuery, DelayRange(0, MaxDelayMilliseconds)] int milliseconds = 500,
            CancellationToken cancellationToken = default)
        {
            await Task.Delay(milliseconds, cancellationToken);

            _logger.LogInformation("Completed delayed response after {Milliseconds} ms.", milliseconds);

            return Ok($"Completed after {milliseconds} ms.");
        }

        /// <summary>
        /// Runs several operations concurrently with Task.WhenAll instead of awaiting them one by one.
        /// </summary>
        [HttpGet("parallel")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetParallelAsync(
            CancellationToken cancellationToken = default)
        {
            Task<string>[] tasks =
            [
                SimulateWorkAsync("users", 300, cancellationToken),
                SimulateWorkAsync("orders", 200, cancellationToken),
                SimulateWorkAsync("inventory", 400, cancellationToken)
            ];

            string[] results = await Task.WhenAll(tasks);

            return Ok(results);
        }

        /// <summary>
        /// Shows the sample Temperature class and its functions in use.
        /// </summary>
        [HttpGet("temperature")]
        public ActionResult<string> GetTemperature([FromQuery] double fahrenheit = 98.6)
        {
            Temperature temperature = Temperature.FromFahrenheit(fahrenheit);

            return Ok(temperature.ToString());
        }

        private static async Task<string> SimulateWorkAsync(
            string name,
            int milliseconds,
            CancellationToken cancellationToken)
        {
            await Task.Delay(milliseconds, cancellationToken);

            return $"{name} loaded in {milliseconds} ms";
        }
    }
}
