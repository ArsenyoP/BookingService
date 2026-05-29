using Booking.Application.DTOs.OpenWeather;

namespace Booking.Application.Interfaces.Services
{
    public interface IOpenWeatherService
    {
        public Task<WeatherResultDto?> GetWeatherByCoordinatesAsync(double latitude, double longitude, string apiKey, CancellationToken ct = default);
        public Task<OpenWeatherGeoDto[]?> GetGeoByCityAsync(string cityName, string apiKey, CancellationToken ct = default);
    }
}
