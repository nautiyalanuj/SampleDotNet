using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestApplication.Controllers;
using TestApplication.Models;
using TestApplication.Services;

namespace TestApplication.Tests;

[TestClass]
public sealed class WeatherForecastControllerTests
{
    private readonly Mock<IWeatherForecastService> _serviceMock = new();

    [TestMethod]
    public void Get_ReturnsOkWithForecastFromService()
    {
        WeatherForecast[] expected =
        [
            new WeatherForecast { Date = new DateOnly(2026, 1, 16), TemperatureC = 20, Summary = "Mild" }
        ];
        _serviceMock.Setup(service => service.GetForecast(It.IsAny<int?>())).Returns(expected);
        var controller = new WeatherForecastController(_serviceMock.Object);

        var response = controller.Get(days: null);

        var okResult = Assert.IsInstanceOfType<OkObjectResult>(response.Result);
        Assert.AreSame(expected, okResult.Value);
        _serviceMock.Verify(service => service.GetForecast(null), Times.Once);
    }

    [TestMethod]
    public void Get_PassesRequestedDaysToTheBusinessLogic()
    {
        var controller = new WeatherForecastController(_serviceMock.Object);

        controller.Get(days: 3);

        _serviceMock.Verify(service => service.GetForecast(3), Times.Once);
    }

    [TestMethod]
    public void Get_WithZeroDays_ReturnsBadRequestWithoutCallingTheBusinessLogic()
    {
        var controller = new WeatherForecastController(_serviceMock.Object);

        var response = controller.Get(days: 0);

        Assert.IsInstanceOfType<BadRequestObjectResult>(response.Result);
        _serviceMock.Verify(service => service.GetForecast(It.IsAny<int?>()), Times.Never);
    }

    [TestMethod]
    public void Get_WithNegativeDays_ReturnsBadRequest()
    {
        var controller = new WeatherForecastController(_serviceMock.Object);

        var response = controller.Get(days: -5);

        Assert.IsInstanceOfType<BadRequestObjectResult>(response.Result);
    }

    [TestMethod]
    public void Constructor_WithNullService_Throws()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => { _ = new WeatherForecastController(null!); });
    }
}