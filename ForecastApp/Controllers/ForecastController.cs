using ForecastApp.Service;
using ForecastApp.Transfer;
using Microsoft.AspNetCore.Mvc;

namespace ForecastApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForecastController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public ForecastController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("GetForecast")]
    public async Task<IActionResult> GetForecastAsync([FromQuery] double latitude, [FromQuery] double longitude)
    {
        var forecast = await _weatherService.GetWeatherForecast(latitude, longitude);

        if (forecast == null)
            return NotFound();

        return Ok(forecast);
    }

    [HttpPost("AddLocation")]
    public async Task<IActionResult> AddLocationAsync([FromBody] LocationDto locationDto)
    {
        var location = await _weatherService.AddLocationAsync(locationDto.Latitude, locationDto.Longitude);

        if (location == null)
            return BadRequest("Unable to create location");

        return Ok(location);
    }

    [HttpGet("GetLocation/{locationId}")]
    public async Task<IActionResult> GetLocationAsync([FromRoute] int locationId)
    {
        var location = await _weatherService.GetLocationAsync(locationId);

        if (location == null)
            return NotFound("Location with provided id not found");

        return Ok(location);
    }

    [HttpGet("GetLocations")]
    public async Task<IActionResult> GetLocationListAsync()
    {
        var locations = await _weatherService.GetAllLocationsAsync();

        if (locations == null || !locations.Any())
            return NotFound("No locations available. Add new locations");

        return Ok(locations);
    }

    [HttpDelete("DeleteLocation/{id}")]
    public async Task<IActionResult> DeleteLocationAsync([FromRoute] int id)
    {
        if (!await _weatherService.DeleteLocationAsync(id))
            return BadRequest("Unable to delete location with provided Id");

        return Ok();
    }

    [HttpGet("GetWeatherByLocation/{id}")]
    public async Task<IActionResult> GetWeatherByLocation([FromRoute] int id)
    {
        var forecast = await _weatherService.GetWeatherByLocationIdAsync(id);

        if (forecast == null)
            return NotFound("Location or forecast not found.");

        return Ok(forecast);
    }
}