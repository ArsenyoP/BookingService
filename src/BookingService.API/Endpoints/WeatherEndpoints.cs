using Booking.Application.UseCases.OpenWeather.GetWeatherByCity;
using MediatR;

namespace Booking.API.Endpoints
{
    public static class WeatherEndpoints
    {
        public static void MapWeatherEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/weather")
                           .RequireRateLimiting("fixed")
                           .WithTags("Weather");

            app.MapGet("/{city:}", GetWeatherByCity);
        }
        private static async Task<IResult> GetWeatherByCity(
            ISender _sender,
            string city,
            CancellationToken ct = default)
        {
            var query = new GetWetherByCityQuery(city);
            var result = await _sender.Send(query, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

    }
}
