using Microsoft.Extensions.Options;
using TestApplication.Configuration;
using TestApplication.Models;

namespace TestApplication.Services
{
    public sealed class WeatherForecastService : IWeatherForecastService
    {
        private readonly WeatherForecastOptions _options;
        private readonly TimeProvider _timeProvider;

        public WeatherForecastService(IOptions<WeatherForecastOptions> options, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _options = options.Value;
            _timeProvider = timeProvider;
        }

        public IReadOnlyList<WeatherForecast> GetForecast(int? days = null)
        {
            var forecastDays = days ?? _options.ForecastDays;

            if (forecastDays <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(days),
                    forecastDays,
                    "The number of forecast days must be greater than zero.");
            }

            var startDate = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);

            return Enumerable.Range(1, forecastDays)
                .Select(offset => new WeatherForecast
                {
                    Date = startDate.AddDays(offset),
                    TemperatureC = Random.Shared.Next(_options.MinTemperatureC, _options.MaxTemperatureC + 1),
                    Summary = _options.Summaries[Random.Shared.Next(_options.Summaries.Length)]
                })
                .ToArray();
        }
    }
}
