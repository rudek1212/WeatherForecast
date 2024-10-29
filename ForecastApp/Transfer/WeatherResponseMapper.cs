using ForecastApp.Model;
using ForecastApp.Response;

namespace ForecastApp.Transfer;

public static class WeatherResponseMapper
{
    public static IEnumerable<WeatherData>? MapToWeatherData(this WeatherHourlyResponse? source, Location location)
    {
        return source?.Time?.Select((time, index) => new WeatherData
        {
            LocationId = location.Id,
            Temperature = source.Temperature2M?[index] ?? 0,
            WindSpeed = source.WindSpeed10M?[index] ?? 0,
            Date = DateTime.Parse(time),
            Location = location
        }).ToList();
    }
}