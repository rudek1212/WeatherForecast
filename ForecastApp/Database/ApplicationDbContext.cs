using ForecastApp.Model;
using Microsoft.EntityFrameworkCore;

namespace ForecastApp.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Location> Locations { get; set; }
    public DbSet<WeatherData> WeatherData { get; set; }
}