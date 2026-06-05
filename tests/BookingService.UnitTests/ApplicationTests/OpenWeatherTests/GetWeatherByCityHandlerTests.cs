using Booking.Application.DTOs.OpenWeather;
using Booking.Application.Interfaces.Services;
using Booking.Application.UseCases.OpenWeather.GetWeatherByCity;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BookingService.UnitTests.ApplicationTests.OpenWeatherTests
{
    public class GetWeatherByCityHandlerTests
    {
        private readonly Mock<IOpenWeatherService> _weatherServiceMock;
        private readonly GetWeatherByCityHandler _handler;
        private readonly Mock<IConfiguration> _config;


        public GetWeatherByCityHandlerTests()
        {
            _weatherServiceMock = new Mock<IOpenWeatherService>();
            _config = new Mock<IConfiguration>();
            _handler = new GetWeatherByCityHandler(_weatherServiceMock.Object, _config.Object);
            _config.Setup(c => c["OpenWeather:ApiKey"]).Returns("mock-api-key");
        }

        [Fact]
        public async Task Handle_ValidCity_ReturnsSuccessWithWeather()
        {
            var city = "Ternopil";
            var mockApiKey = "mock-api-key";
            var query = new GetWetherByCityQuery(city);

            var geoMock = new[] { new OpenWeatherGeoDto { Latitude = 49.55, Longitude = 25.59, CityName = "Ternopil" } };
            var weatherMock = new WeatherResultDto
            {
                CityName = "Ternopil",
                Temperature = 22.5m,
                CloudinessPercent = 20,
                Description = "clear sky",
                Main = "Clear",
                WindSpeed = 3.5m
            };

            _weatherServiceMock.Setup(x => x.GetGeoByCityAsync(city, mockApiKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(geoMock);

            _weatherServiceMock.Setup(x => x.GetWeatherByCoordinatesAsync(geoMock[0].Latitude, geoMock[0].Longitude, mockApiKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(weatherMock);


            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.CityName.Should().Be("Ternopil");
            result.Value.Temperature.Should().Be(22.5m);
        }

        [Fact]
        public async Task Handle_NullCoordinates_ReturnsNotFound()
        {
            var city = "Ternopil";
            var mockApiKey = "mock-api-key";
            var geoMock = new OpenWeatherGeoDto[] { null! };
            var query = new GetWetherByCityQuery(city);


            _weatherServiceMock.Setup(x => x.GetGeoByCityAsync(city, mockApiKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(geoMock);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("Geo.CityNotFounf");
        }
    }
}
