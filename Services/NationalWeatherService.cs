using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherApi.Entity;

namespace WeatherApi.Services
{
    public class NationalWeatherService
    {
        private const string baseUrl = "https://api.weather.gov/";
        private readonly Dictionary<string, int> tempThresholdMap = new Dictionary<string, int>
        {
            { "Freezing", 33 },
            { "Cold", 55 },
            { "Cool", 70 },
            { "Warm", 80 },
            { "Hot", 170 }
        };

        private static readonly HttpClient client = new HttpClient();

        private struct GridInfo
        {
            public string GridX { get; set; }
            public string GridY { get; set; }
            public string GridId {  get; set; }
        }

        private struct ForecastInfo
        {
            public string ShortForecast { get; set; }
            public int Temperature { get; set; }
            public string TemperatureUnit { get; set; }
        }

        static NationalWeatherService()
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
        }

        public async Task<WeatherInfo> GetCurrentWeather(double latitude, double longitude)
        {
            var gridInfo = await this.GetGrid(latitude, longitude);
            var forecast = await this.GetForecast(gridInfo);

            return new WeatherInfo
            {
                ShortForecast = forecast.ShortForecast,
                Characteristic = this.mapTemperatureToCharacteristic(
                    forecast.Temperature,
                    forecast.TemperatureUnit
                )
            };
        }

        private async Task<GridInfo> GetGrid(double latitude, double longitude)
        {
            using var response = await client.GetAsync($"points/{latitude},{longitude}");
            response.EnsureSuccessStatusCode();

            string rawJson = await response.Content.ReadAsStringAsync();
            JObject keyValuePairs = JObject.Parse(rawJson);
            JToken? properties = keyValuePairs["properties"];

            if (properties != null)
            {
                return properties.ToObject<GridInfo>();
            }

            throw new Exception("Failed to read wheather grid info from given coordinates");
        }

        private async Task<ForecastInfo> GetForecast(GridInfo info)
        {
            using var response = await client.GetAsync(
                $"gridpoints/{info.GridId}/{info.GridX},{info.GridY}/forecast"
            );
            response.EnsureSuccessStatusCode();
            string rawJson = await response.Content.ReadAsStringAsync();
            JObject keyValuePairs = JObject.Parse(rawJson);

            JToken? currentForecast = keyValuePairs["properties"]?["periods"]?[0];
            if (currentForecast != null)
            {
                return currentForecast.ToObject<ForecastInfo>();
            }

            throw new Exception("Failed to read forecast info for provided grid information.");
        }

        private string mapTemperatureToCharacteristic(int temperature, string temperatureUnit)
        {
            if (temperatureUnit == "C")
            {
                temperature = 32 + (int) (temperature / 0.55556);
            }
          
            foreach (var item in this.tempThresholdMap)
            {
                if (temperature <= item.Value)
                {
                    return item.Key;
                }
            }

            // TODO: log and/or throw
            return "Unknown";
        }
    }
}
