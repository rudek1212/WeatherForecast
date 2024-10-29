using System.Text.Json.Serialization;

namespace ForecastApp.Response;

public class ForecastResponse
{
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }
    
    [JsonPropertyName("hourly")]
    public WeatherHourlyResponse? Hourly { get; set; }
}

public class WeatherHourlyResponse
{
    [JsonPropertyName("time")]

    public List<string>? Time { get; set; }

    [JsonPropertyName("temperature_2m")] 
    public List<double>? Temperature2M { get; set; }

    [JsonPropertyName("wind_speed_10m")] 
    public List<double>? WindSpeed10M { get; set; }
}