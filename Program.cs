using FirstApplication.Configuration;
using FirstApplication.Middleware;
using FirstApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddOptions<WeatherForecastOptions>()
    .Bind(builder.Configuration.GetSection(WeatherForecastOptions.SectionName))
    .Validate(
        options => options.ForecastDays > 0,
        "WeatherForecast:ForecastDays must be greater than zero.")
    .Validate(
        options => options.MinTemperatureC <= options.MaxTemperatureC,
        "WeatherForecast:MinTemperatureC must be less than or equal to WeatherForecast:MaxTemperatureC.")
    .Validate(
        options => options.Summaries.Length > 0,
        "WeatherForecast:Summaries must contain at least one entry.")
    .ValidateOnStart();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

var app = builder.Build();

app.UseApiResponseLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
