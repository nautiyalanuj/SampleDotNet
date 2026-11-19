using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FirstApplication.Configuration;
using FirstApplication.Services;

namespace FirstApplication.Tests;

[TestClass]
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

    [TestMethod]
    public void GetForecast_UsesConfiguredForecastDays()
    {
        var options = new WeatherForecastOptions { ForecastDays = 3 };

        var forecast = CreateService(options).GetForecast();

        Assert.HasCount(3, forecast);
    }

    [TestMethod]
    public void GetForecast_StartsTheDayAfterTheCurrentUtcDateAndIncrementsDaily()
    {
        var options = new WeatherForecastOptions { ForecastDays = 3 };

        var forecast = CreateService(options).GetForecast();

        Assert.AreEqual(new DateOnly(2026, 1, 16), forecast[0].Date);
        Assert.AreEqual(new DateOnly(2026, 1, 17), forecast[1].Date);
        Assert.AreEqual(new DateOnly(2026, 1, 18), forecast[2].Date);
    }

    [TestMethod]
    public void GetForecast_WithExplicitDays_OverridesConfiguredValue()
    {
        var options = new WeatherForecastOptions { ForecastDays = 2 };

        var forecast = CreateService(options).GetForecast(days: 6);

        Assert.HasCount(6, forecast);
    }

    [TestMethod]
    public void GetForecast_KeepsTemperaturesWithinConfiguredRange()
    {
        var options = new WeatherForecastOptions
        {
            ForecastDays = 50,
            MinTemperatureC = 10,
            MaxTemperatureC = 12
        };

        var forecast = CreateService(options).GetForecast();

        Assert.IsTrue(
            forecast.All(entry => entry.TemperatureC is >= 10 and <= 12),
            "Every temperature must stay inside the configured range.");
    }

    [TestMethod]
    public void GetForecast_UsesConfiguredSummaries()
    {
        var options = new WeatherForecastOptions
        {
            ForecastDays = 5,
            Summaries = ["Sunny"]
        };

        var forecast = CreateService(options).GetForecast();

        Assert.IsTrue(
            forecast.All(entry => entry.Summary == "Sunny"),
            "Every summary must come from the configured list.");
    }

    [TestMethod]
    public void GetForecast_WithZeroDays_Throws()
    {
        var service = CreateService(new WeatherForecastOptions());

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => { service.GetForecast(0); });
    }

    [TestMethod]
    public void GetForecast_WithNegativeDays_Throws()
    {
        var service = CreateService(new WeatherForecastOptions());

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => { service.GetForecast(-1); });
    }

    [TestMethod]
    public void Constructor_WithNullOptions_Throws()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => { _ = new WeatherForecastService(null!, Mock.Of<TimeProvider>()); });
    }

    [TestMethod]
    public void Constructor_WithNullTimeProvider_Throws()
    {
        var options = Mock.Of<IOptions<WeatherForecastOptions>>();

        Assert.ThrowsExactly<ArgumentNullException>(
            () => { _ = new WeatherForecastService(options, null!); });
    }
}