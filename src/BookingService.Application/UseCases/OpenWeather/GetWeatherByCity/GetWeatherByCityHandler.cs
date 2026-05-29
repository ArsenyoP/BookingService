using Booking.Application.Abstractions;
using Booking.Application.DTOs.OpenWeather;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;
using Microsoft.Extensions.Configuration;

namespace Booking.Application.UseCases.OpenWeather.GetWeatherByCity
{
    public sealed class GetWeatherByCityHandler(IOpenWeatherService _weatherService,
        IConfiguration _config)
        : IQueryHandler<GetWetherByCityQuery, WeatherResultDto>
    {
        public async Task<Result<WeatherResultDto>> Handle(GetWetherByCityQuery request, CancellationToken ct)
        {
            var key = _config["OpenWeather:ApiKey"];

            var geo = await _weatherService.GetGeoByCityAsync(request.City, key!, ct);

            if (geo is null || geo.Length == 0)
            {
                return Result<WeatherResultDto>.Failure(new Error("Geo.CityNotFounf", "Specified city was not found"));
            }

            var coordinates = geo.FirstOrDefault();

            if (coordinates is null) return Result<WeatherResultDto>.Failure(new Error("Geo.CityNotFounf", "Specified city was not found"));

            var weather = await _weatherService.GetWeatherByCoordinatesAsync(
                coordinates.Latitude,
                coordinates.Longitude,
                key!,
                ct);



            if (weather is null) return Result<WeatherResultDto>.Failure(new Error("Weather.CityNotFounf", "Weather was not found"));

            var weatherResponse = new WeatherResultDto
            {
                CityName = weather.CityName,
                Temperature = weather.Temperature,
                CloudinessPercent = weather.CloudinessPercent,
                Description = weather.Description,
                Main = weather.Main,
                WindSpeed = weather.WindSpeed
            };

            return Result<WeatherResultDto>.Success(weatherResponse);
        }
    }
}
