using ForecastApp.Database;
using ForecastApp.Service;
using Microsoft.EntityFrameworkCore;

namespace ForecastApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddWeatherAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IWeatherService, WeatherService>();

        services.RegisterDatabase();

        services.AddScoped<IWeatherService, WeatherService>();

        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void RegisterDatabase(this IServiceCollection services)
    {
        var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        if (!Directory.Exists(dataDirectory))
            Directory.CreateDirectory(dataDirectory);

        var databasePath = Path.Combine(dataDirectory, "weather.db");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));
    }
}