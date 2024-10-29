namespace ForecastApp.Model;

public class WeatherData
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public DateTime Date { get; set; }

    public Location Location { get; set; }
}