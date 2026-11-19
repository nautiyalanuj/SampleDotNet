using Microsoft.AspNetCore.Mvc;
using FirstApplication.Models;
using FirstApplication.Services;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastController(IWeatherForecastService weatherForecastService)
        {
            ArgumentNullException.ThrowIfNull(weatherForecastService);

            _weatherForecastService = weatherForecastService;
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<WeatherForecast>> Get([FromQuery] int? days)
        {
            if (days <= 0)
            {
                return BadRequest("days must be greater than zero.");
            }

            return Ok(_weatherForecastService.GetForecast(days));
        }
    }
}
