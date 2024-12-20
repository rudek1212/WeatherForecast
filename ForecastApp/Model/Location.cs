﻿namespace ForecastApp.Model;

public class Location
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public List<WeatherData>? WeatherData { get; set; } = new();
}