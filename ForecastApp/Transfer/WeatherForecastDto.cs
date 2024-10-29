namespace ForecastApp.Transfer;

public class WeatherForecastDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public IEnumerable<WeatherDataDto>? WeatherData { get; set; }
}