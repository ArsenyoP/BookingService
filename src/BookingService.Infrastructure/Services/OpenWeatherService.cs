using Booking.Application.DTOs.OpenWeather;
using Booking.Application.Interfaces.Services;
using System.Net.Http.Json;

namespace Booking.Infrastructure.Services
{
    public sealed class OpenWeatherService(HttpClient _httpClient)
        : IOpenWeatherService
    {
        public async Task<OpenWeatherGeoDto[]?> GetGeoByCityAsync(string cityName, string apiKey, CancellationToken ct = default)
        {
            var baseUrl = "http://api.openweathermap.org";
            var result = await _httpClient.GetFromJsonAsync<OpenWeatherGeoDto[]>($"{baseUrl}/geo/1.0/direct?q={cityName}&limit=1&appid={apiKey}");
            return result;
        }

        public async Task<WeatherResultDto?> GetWeatherByCoordinatesAsync(double latitude, double longitude, string apiKey, CancellationToken ct = default)
        {
            var baseUrl = "http://api.openweathermap.org";

            var url = $"{baseUrl}/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&lang=uk";

            var response = await _httpClient.GetFromJsonAsync<OpenWeatherResponse>(url, ct);

            if (response is null) return null;

            var weatherInfo = response.Weather.FirstOrDefault();

            var result = new WeatherResultDto
            {
                CityName = response.CityName,
                Temperature = response.Main.Temp,
                CloudinessPercent = response.Clouds.All,
                WindSpeed = response.Wind.Speed,
                Main = weatherInfo?.Main ?? string.Empty,
                Description = weatherInfo?.Description ?? string.Empty
            };

            return result;
        }
    }
}
