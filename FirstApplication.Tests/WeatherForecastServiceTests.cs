using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using FirstApplication.Configuration;
using FirstApplication.Services;

namespace FirstApplication.Tests;

public sealed class WeatherForecastServiceTests
{
    private static readonly DateTimeOffset FixedUtcNow = new(2026, 1, 15, 23, 30, 0, TimeSpan.Zero);

    private static WeatherForecastService CreateService(WeatherForecastOptions options, DateTimeOffset? utcNow = null)
    {
        var optionsMock = new Mock<IOptions<WeatherForecastOptions>>();
        optionsMock.SetupGet(options => options.Value).Returns(options);

        var timeProviderMock = new Mock<TimeProvider>();
        timeProviderMock.Setup(timeProvider => timeProvider.GetUtcNow()).Returns(utcNow ?? FixedUtcNow);

        return new WeatherForecastService(optionsMock.Object, timeProviderMock.Object);
    }

    [Fact]
    public void GetForecast_UsesConfiguredForecastDays()
    {
        var options = new WeatherForecastOptions { ForecastDays = 3 };

        var forecast = CreateService(options).GetForecast();

        Assert.Equal(3, forecast.Count);
    }

    [Fact]
    public void GetForecast_StartsTheDayAfterTheCurrentUtcDateAndIncrementsDaily()
    {
        var options = new WeatherForecastOptions { ForecastDays = 3 };

        var forecast = CreateService(options).GetForecast();

        Assert.Equal(new DateOnly(2026, 1, 16), forecast[0].Date);
        Assert.Equal(new DateOnly(2026, 1, 17), forecast[1].Date);
        Assert.Equal(new DateOnly(2026, 1, 18), forecast[2].Date);
    }

    [Fact]
    public void GetForecast_WithExplicitDays_OverridesConfiguredValue()
    {
        var options = new WeatherForecastOptions { ForecastDays = 2 };

        var forecast = CreateService(options).GetForecast(days: 6);

        Assert.Equal(6, forecast.Count);
    }

    [Fact]
    public void GetForecast_KeepsTemperaturesWithinConfiguredRange()
    {
        var options = new WeatherForecastOptions
        {
            ForecastDays = 50,
            MinTemperatureC = 10,
            MaxTemperatureC = 12
        };

        var forecast = CreateService(options).GetForecast();

        Assert.True(
            forecast.All(entry => entry.TemperatureC is >= 10 and <= 12),
            "Every temperature must stay inside the configured range.");
    }

    [Fact]
    public void GetForecast_UsesConfiguredSummaries()
    {
        var options = new WeatherForecastOptions
        {
            ForecastDays = 5,
            Summaries = ["Sunny"]
        };

        var forecast = CreateService(options).GetForecast();

        Assert.True(
            forecast.All(entry => entry.Summary == "Sunny"),
            "Every summary must come from the configured list.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetForecast_WithZeroOrNegativeDays_Throws(int days)
    {
        var service = CreateService(new WeatherForecastOptions());

        Assert.Throws<ArgumentOutOfRangeException>(() => { service.GetForecast(days); });
    }

    [Fact]
    public void Constructor_WithNullOptions_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => { _ = new WeatherForecastService(null!, Mock.Of<TimeProvider>()); });
    }

    [Fact]
    public void Constructor_WithNullTimeProvider_Throws()
    {
        var options = Mock.Of<IOptions<WeatherForecastOptions>>();

        Assert.Throws<ArgumentNullException>(
            () => { _ = new WeatherForecastService(options, null!); });
    }
}
