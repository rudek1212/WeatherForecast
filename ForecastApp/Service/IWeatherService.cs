using ForecastApp.Transfer;

namespace ForecastApp.Service;

public interface IWeatherService
{
    Task<WeatherForecastDto?> GetWeatherForecast(double latitude, double longitude, CancellationToken cancellationToken = default);
    Task<LocationDto?> AddLocationAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
    Task<LocationDto?> GetLocationAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteLocationAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LocationDto>?> GetAllLocationsAsync(CancellationToken cancellationToken = default);
    Task<WeatherForecastDto?> GetWeatherByLocationIdAsync(int id, CancellationToken cancellationToken = default);
}