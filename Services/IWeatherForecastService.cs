using TestApplication.Models;

namespace TestApplication.Services
{
    public interface IWeatherForecastService
    {
        /// <summary>
        /// Builds a forecast starting the day after the current UTC date.
        /// </summary>
        /// <param name="days">Number of days to forecast; defaults to the configured <c>ForecastDays</c>.</param>
        IReadOnlyList<WeatherForecast> GetForecast(int? days = null);
    }
}
