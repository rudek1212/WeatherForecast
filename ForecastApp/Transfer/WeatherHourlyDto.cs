using System.Text.Json.Serialization;

namespace ForecastApp.Transfer;

public class WeatherHourlyDto
{
    public List<string>? Time { get; set; }

    [JsonPropertyName("Temperature_2m")] public List<double>? Temperature2M { get; set; }

    [JsonPropertyName("WindSpeed_10m")] public List<double>? WindSpeed10M { get; set; }
}