using Microsoft.AspNetCore.Mvc;
using WeatherApi.Entity;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/currentWeather/{latitude},{longitude}", Name = "GetCurrentWeather")]
        public async Task<WeatherInfo> GetCurrentWeather(double latitude = 39.7456, double longitude = -97.0892)
        {
            var service = new NationalWeatherService();

            return await service.GetCurrentWeather(latitude, longitude);
        }
    }
}
