using Booking.Application.Abstractions;
using Booking.Application.DTOs.OpenWeather;

namespace Booking.Application.UseCases.OpenWeather.GetWeatherByCity
{
    public sealed record GetWetherByCityQuery(string City)
        : IQuery<WeatherResultDto>;
}
