namespace TestApplication.Configuration
{
    /// <summary>
    /// Configuration bound from the "WeatherForecast" section and consumed by the business logic.
    /// </summary>
    public sealed class WeatherForecastOptions
    {
        public const string SectionName = "WeatherForecast";

        public int ForecastDays { get; set; } = 5;

        public int MinTemperatureC { get; set; } = -20;

        public int MaxTemperatureC { get; set; } = 55;

        public string[] Summaries { get; set; } =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];
    }
}
