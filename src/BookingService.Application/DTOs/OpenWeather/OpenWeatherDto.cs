using System.Text.Json.Serialization;

namespace Booking.Application.DTOs.OpenWeather
{
    public record OpenWeatherResponse
    {
        [JsonPropertyName("name")]
        public string CityName { get; init; } = string.Empty;

        [JsonPropertyName("weather")]
        public List<WeatherDescriptionData> Weather { get; init; } = [];

        [JsonPropertyName("main")]
        public MainData Main { get; init; } = new();

        [JsonPropertyName("wind")]
        public WindData Wind { get; init; } = new();

        [JsonPropertyName("clouds")]
        public CloudsData Clouds { get; init; } = new();
    }

    public record WeatherDescriptionData
    {
        [JsonPropertyName("main")]
        public string Main { get; init; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; init; } = string.Empty;
    }

    public record MainData
    {
        [JsonPropertyName("temp")]
        public decimal Temp { get; init; }
    }

    public record WindData
    {
        [JsonPropertyName("speed")]
        public decimal Speed { get; init; }
    }

    public record CloudsData
    {
        [JsonPropertyName("all")]
        public int All { get; init; }
    }



    public record WeatherResultDto
    {
        public string CityName { get; init; } = string.Empty;
        public decimal Temperature { get; init; }
        public int CloudinessPercent { get; init; }
        public decimal WindSpeed { get; init; }
        public string Main { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }
}
