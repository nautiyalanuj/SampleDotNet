using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FirstApplication.Controllers;
using FirstApplication.Models;
using FirstApplication.Services;

namespace FirstApplication.Tests;

public sealed class WeatherForecastControllerTests
{
    private readonly Mock<IWeatherForecastService> _serviceMock = new();

    [Fact]
    public void Get_ReturnsOkWithForecastFromService()
    {
        WeatherForecast[] expected =
        [
            new WeatherForecast { Date = new DateOnly(2026, 1, 16), TemperatureC = 20, Summary = "Mild" }
        ];
        _serviceMock.Setup(service => service.GetForecast(It.IsAny<int?>())).Returns(expected);
        var controller = new WeatherForecastController(_serviceMock.Object);

        var response = controller.Get(days: null);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(expected, okResult.Value);
        _serviceMock.Verify(service => service.GetForecast(null), Times.Once);
    }

    [Fact]
    public void Get_PassesRequestedDaysToTheBusinessLogic()
    {
        var controller = new WeatherForecastController(_serviceMock.Object);

        controller.Get(days: 3);

        _serviceMock.Verify(service => service.GetForecast(3), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Get_WithZeroOrNegativeDays_ReturnsBadRequestWithoutCallingTheBusinessLogic(int days)
    {
        var controller = new WeatherForecastController(_serviceMock.Object);

        var response = controller.Get(days: days);

        Assert.IsType<BadRequestObjectResult>(response.Result);
        _serviceMock.Verify(service => service.GetForecast(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public void Constructor_WithNullService_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => { _ = new WeatherForecastController(null!); });
    }
}
