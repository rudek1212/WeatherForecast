using ForecastApp.Database;
using ForecastApp.Model;
using ForecastApp.Response;
using ForecastApp.Transfer;
using Microsoft.EntityFrameworkCore;

namespace ForecastApp.Service;

public class WeatherService : IWeatherService
{
    private static int _forecastDaysTolerance = 24;

    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient, ApplicationDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public async Task<WeatherForecastDto?> GetWeatherForecast(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var location = await _context.Locations
                .Include(x => x.WeatherData)
                .Where(x =>
                    x.Latitude == latitude &&
                    x.Longitude == longitude)
                .FirstOrDefaultAsync(cancellationToken);

            if (location == null)
            {
                location = new Location
                {
                    Latitude = latitude,
                    Longitude = longitude
                };

                await _context.Locations.AddAsync(location, cancellationToken);
            }

            if (location.WeatherData != null &&
                location.WeatherData.Any() &&
                location.WeatherData.Count(x => x.Date <= DateTime.UtcNow) < _forecastDaysTolerance)
                return new WeatherForecastDto
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    WeatherData = location.WeatherData.Select(
                        x => new WeatherDataDto
                        {
                            Date = x.Date,
                            Temperature = x.Temperature,
                            WindSpeed = x.WindSpeed
                        })
                };

            var forecast = await FetchWeatherForecastAsync(latitude, longitude, cancellationToken);

            if (forecast == null)
                throw new Exception("Forecast unavailable for provided location.");

            var weatherDataList = forecast.Hourly?.MapToWeatherData(location);

            if (weatherDataList != null)
                _context.WeatherData.AddRange(weatherDataList);

            await transaction.CommitAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return location.MapToWeatherForecastDto();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return null;
        }
    }

    public async Task<LocationDto?> AddLocationAsync(double latitude, double longitude,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _context.Locations.AnyAsync(x => x.Latitude == latitude && x.Longitude == longitude, cancellationToken))
                return null;

            var location = new Location
            {
                Latitude = latitude,
                Longitude = longitude
            };

            await _context.AddAsync(location, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return location.MapToDto();
        }
        catch (Exception e)
        {
            return null;
        }


    }

    public async Task<LocationDto?> GetLocationAsync(int id, CancellationToken cancellationToken = default)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return location?.MapToDto();
    }

    public async Task<bool> DeleteLocationAsync(int id, CancellationToken cancellationToken = default)
    {
        var location =
            await _context.Locations
                .Include(x => x.WeatherData)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (location == null)
            return false;

        _context.Remove(location);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<LocationDto>?> GetAllLocationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Locations.Select(l => l.MapToDto()).ToArrayAsync(cancellationToken);
    }

    public async Task<WeatherForecastDto?> GetWeatherByLocationIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var location = await _context.Locations
            .Include(x => x.WeatherData)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (location == null)
            return null;

        if (location.WeatherData != null &&
            location.WeatherData.Any() &&
            location.WeatherData.Count(x => x.Date <= DateTime.UtcNow) < _forecastDaysTolerance)
            return new WeatherForecastDto
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                WeatherData = location.WeatherData.Select(
                    x => new WeatherDataDto
                    {
                        Date = x.Date,
                        Temperature = x.Temperature,
                        WindSpeed = x.WindSpeed
                    })
            };

        var forecast = await FetchWeatherForecastAsync(location.Latitude, location.Longitude, cancellationToken);

        if (forecast == null)
            throw new Exception("Forecast unavailable for provided location.");

        var weatherDataList = forecast.Hourly?.MapToWeatherData(location);

        if (weatherDataList != null)
            _context.WeatherData.AddRange(weatherDataList);

        return location?.MapToWeatherForecastDto();
    }

    private async Task<ForecastResponse?> FetchWeatherForecastAsync(
        double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var url =
            $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&timezone=UTC&hourly=temperature_2m,wind_speed_10m";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode) return null;

        var forecastData = await response.Content.ReadFromJsonAsync<ForecastResponse>(cancellationToken);
        return forecastData;
    }
}