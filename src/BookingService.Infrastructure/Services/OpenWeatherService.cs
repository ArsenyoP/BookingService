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

        public async Task<OpenWeatherDto[]?> GetWeatherByCoordinatesAsync(string latitude, string longitude, string apiKey, CancellationToken ct = default)
        {
            var baseUrl = "http://api.openweathermap.org";
            var result = await _httpClient.GetFromJsonAsync<OpenWeatherDto[]>($"{baseUrl}/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}");
            return result;
        }
    }
}
