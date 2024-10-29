using ForecastApp.Extensions;

namespace ForecastApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddWeatherAppServices(builder.Configuration);

        var app = builder.Build();

        app.ConfigureWeatherApp(app.Environment);

        app.Run();
    }
}