namespace ForecastApp.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ConfigureWeatherApp(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        return app;
    }
}