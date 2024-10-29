using ForecastApp.Model;
using ForecastApp.Response;

namespace ForecastApp.Transfer;

public static class WeatherDataDtoMapper
{
    public static LocationDto MapToDto(this Location source)
    {
        return new LocationDto
        {
            Id = source.Id,
            Latitude = source.Latitude,
            Longitude = source.Longitude
        };
    }

    public static WeatherForecastDto? MapToWeatherForecastDto(this Location? source)
    {
        if (source == null)
            return null;

        return new WeatherForecastDto
        {
            Longitude = source.Longitude,
            Latitude = source.Latitude,
            WeatherData = source.WeatherData?.Select(x => new WeatherDataDto
            {
                Date = x.Date,
                Temperature = x.Temperature,
                WindSpeed = x.WindSpeed
            })
        };
    }

    public static WeatherForecastDto? MapToWeatherForecastDto(this ForecastResponse? source)
    {
        if (source == null)
            return null;

        return new WeatherForecastDto
        {
            Latitude = source.Latitude.GetValueOrDefault(),
            Longitude = source.Longitude.GetValueOrDefault(),
            WeatherData = source.Hourly?.Time?.Select((time, index) => new WeatherDataDto
            {
                Temperature = source.Hourly.Temperature2M?[index] ?? 0,
                WindSpeed = source.Hourly.WindSpeed10M?[index] ?? 0,
                Date = DateTime.Parse(time)
            }).ToList()
        };
    }
}