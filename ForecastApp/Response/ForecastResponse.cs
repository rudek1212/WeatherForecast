using System.Text.Json.Serialization;

namespace ForecastApp.Response;

public class ForecastResponse
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public WeatherHourlyResponse? Hourly { get; set; }
}

public class WeatherHourlyResponse
{
    public List<string>? Time { get; set; }

    [JsonPropertyName("Temperature_2m")] public List<double>? Temperature2M { get; set; }

    [JsonPropertyName("WindSpeed_10m")] public List<double>? WindSpeed10M { get; set; }
}