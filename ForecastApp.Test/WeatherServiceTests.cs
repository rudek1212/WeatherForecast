using System.Net;
using System.Text.Json;
using ForecastApp.Database;
using ForecastApp.Model;
using ForecastApp.Response;
using ForecastApp.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Moq.Protected;

namespace ForecastApp.Test
{
    public class WeatherServiceTests
    {
        private readonly WeatherService _weatherService;
        private readonly ApplicationDbContext _context;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public WeatherServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ForecastAppTestDb")
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _httpMessageHandlerMock = HttpClientMockHelper.CreateMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new ForecastResponse
                {
                    Latitude = 55.7558,
                    Longitude = 37.6176,
                    Hourly = new WeatherHourlyResponse
                    {
                        Time = new List<string> { DateTime.UtcNow.ToString("o") },
                        Temperature2M = new List<double> { 10 },
                        WindSpeed10M = new List<double> { 5 }
                    }
                }))
            });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _weatherService = new WeatherService(httpClient, _context);
        }

        [Fact]
        public async Task GetWeatherForecast_ShouldReturnForecast_WhenLocationExists()
        {
            // Arrange
            var latitude = 40.7128;
            var longitude = -74.0060;

            var location = new Location
            {
                Latitude = latitude,
                Longitude = longitude,
                WeatherData = new List<WeatherData>
                {
                    new WeatherData
                    {
                        Date = DateTime.UtcNow,
                        Temperature = 25,
                        WindSpeed = 5,
                    }
                }
            };

            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();

            // Act
            var result = await _weatherService.GetWeatherForecast(latitude, longitude);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(latitude, result.Latitude);
            Assert.Equal(longitude, result.Longitude);
            Assert.Single(result.WeatherData);
        }

        [Fact]
        public async Task AddLocationAsync_ShouldReturnLocationDto_WhenLocationIsNew()
        {
            // Arrange
            var latitude = 51.5074;
            var longitude = -0.1278;

            // Act
            var result = await _weatherService.AddLocationAsync(latitude, longitude);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(latitude, result.Latitude);
            Assert.Equal(longitude, result.Longitude);
        }

        [Fact]
        public async Task DeleteLocationAsync_ShouldReturnTrue_WhenLocationIsDeleted()
        {
            // Arrange
            var location = new Location { Latitude = 35.6895, Longitude = 139.6917 };
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();

            // Act
            var result = await _weatherService.DeleteLocationAsync(location.Id);

            // Assert
            Assert.True(result);
            Assert.Null(await _context.Locations.FindAsync(location.Id));
        }

        [Fact]
        public async Task GetWeatherByLocationIdAsync_ShouldFetchAndSaveForecast_WhenWeatherDataDoesNotExist()
        {
            // Arrange
            var location = new Location { Latitude = 55.7558, Longitude = 37.6176 };
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();

            // Act
            var result = await _weatherService.GetWeatherByLocationIdAsync(location.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(location.Latitude, result.Latitude);
            Assert.Equal(location.Longitude, result.Longitude);
            Assert.Single(result.WeatherData);

            // Verify that the mocked handler was used
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public void ForecastResponse_ShouldDeserializeCorrectly()
        {
            // Arrange
            string json = @"
            {
                ""latitude"": 49.9375,
                ""longitude"": 50.0,
                ""generationtime_ms"": 0.36203861236572266,
                ""utc_offset_seconds"": 0,
                ""timezone"": ""UTC"",
                ""timezone_abbreviation"": ""UTC"",
                ""elevation"": 6.0,
                ""hourly_units"": {
                    ""time"": ""iso8601"",
                    ""temperature_2m"": ""°C"",
                    ""wind_speed_10m"": ""km/h""
                },
                ""hourly"": {
                    ""time"": [""2024-10-29T00:00"", ""2024-10-29T01:00""],
                    ""temperature_2m"": [6.2, 5.7],
                    ""wind_speed_10m"": [4.0, 4.7]
                }
            }";

            // Act
            var forecastResponse = JsonSerializer.Deserialize<ForecastResponse>(json);

            // Assert
            Assert.NotNull(forecastResponse); 
            Assert.Equal(49.9375, forecastResponse.Latitude);
            Assert.Equal(50.0, forecastResponse.Longitude);
            Assert.NotNull(forecastResponse.Hourly);
            Assert.Equal(2, forecastResponse.Hourly.Time.Count);
            Assert.Equal(6.2, forecastResponse.Hourly.Temperature2M[0]);
            Assert.Equal(4.0, forecastResponse.Hourly.WindSpeed10M[0]);
        }

        [Fact]
        public async Task AddLocationAsync_ShouldReturnNull_WhenLocationAlreadyExists()
        {
            // Arrange
            var latitude = 51.5074;
            var longitude = -0.1278;
            await _weatherService.AddLocationAsync(latitude, longitude);

            // Act
            var result = await _weatherService.AddLocationAsync(latitude, longitude);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteLocationAsync_ShouldReturnFalse_WhenLocationDoesNotExist()
        {
            // Act
            var result = await _weatherService.DeleteLocationAsync(9999); // Use a non-existent ID

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetWeatherForecast_ShouldHandleApiFailureGracefully()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var latitude = 55.7558;
            var longitude = 37.6176;

            // Act
            var result = await _weatherService.GetWeatherForecast(latitude, longitude);

            // Assert
            Assert.Null(result);  // Expecting null due to API failure
        }

    }
}
