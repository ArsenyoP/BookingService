using System.Text.Json.Serialization;

namespace Booking.Application.DTOs.OpenWeather
{
    public sealed record OpenWeatherGeoDto
    {
        [JsonPropertyName("lon")]
        public double Longitude { get; set; }

        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        [JsonPropertyName("name")]
        public string CityName { get; set; } = string.Empty;
    }
}
