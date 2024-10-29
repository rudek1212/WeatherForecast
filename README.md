# ForecastApp

## Overview

**ForecastApp** is a web application providing weather forecasts based on user-provided geographic coordinates. Built using .NET and ASP.NET Core, it leverages the Open-Meteo API to fetch weather data and stores location and weather history using Entity Framework Core.

This project includes:
- An API to fetch weather data based on latitude and longitude
- CRUD operations for managing favorite locations
- Cached weather data for previously searched locations

## Features

- **Retrieve Weather Forecast**: Get a forecast based on latitude and longitude.
- **Save Locations**: Save specific locations for future reference.
- **Retrieve Location by ID**: Retrieve saved locations by unique ID.
- **Delete Locations**: Remove a location from storage.
- **List All Locations**: List all saved locations.
- **Fetch Weather for Saved Locations**: Retrieve current weather data for any saved location.

## API Endpoints

### Base URL
`/api/forecast`


### Endpoints

1. **Get Weather Forecast**

   - **Endpoint**: `GET /api/forecast/GetForecast`
   - **Parameters**: `latitude` (double), `longitude` (double)
   - **Description**: Retrieves weather data for the specified coordinates.
   - **Response**: Weather data for the specified latitude and longitude, or `404` if unavailable.

2. **Add a New Location**

   - **Endpoint**: `POST /api/forecast/AddLocation`
   - **Body**:
     ```json
     {
       "latitude": double,
       "longitude": double
     }
     ```
   - **Description**: Adds a new location to the database.
   - **Response**: Newly created location data or `400` if the location already exists or could not be created.

3. **Get Location by ID**

   - **Endpoint**: `GET /api/forecast/GetLocation/{locationId}`
   - **Parameters**: `locationId` (int)
   - **Description**: Retrieves details of a saved location by ID.
   - **Response**: Location data for the specified ID, or `404` if not found.

4. **Get All Locations**

   - **Endpoint**: `GET /api/forecast/GetLocations`
   - **Description**: Lists all stored locations.
   - **Response**: Array of locations or `404` if no locations are found.

5. **Delete a Location**

   - **Endpoint**: `DELETE /api/forecast/DeleteLocation/{id}`
   - **Parameters**: `id` (int)
   - **Description**: Deletes a specified location by its ID.
   - **Response**: `200 OK` if successful, or `400` if deletion fails.

6. **Get Weather by Location ID**

   - **Endpoint**: `GET /api/forecast/GetWeatherByLocation/{id}`
   - **Parameters**: `id` (int)
   - **Description**: Retrieves weather data for a saved location by ID.
   - **Response**: Weather forecast data or `404` if the location or forecast is unavailable.

## Models

### Location

- **Id**: Unique identifier
- **Latitude**: Latitude of the location
- **Longitude**: Longitude of the location
- **WeatherData**: List of associated weather data records

### WeatherData

- **Id**: Unique identifier
- **LocationId**: Foreign key referencing `Location`
- **Temperature**: Temperature value for the location at the recorded time
- **WindSpeed**: Wind speed for the location at the recorded time
- **Date**: Timestamp of the weather data

## Services

### WeatherService

The core service of the app that manages weather data retrieval and caching for locations.

#### Key Methods

- **GetWeatherForecast**: Fetches and caches weather forecast for specified coordinates.
- **AddLocationAsync**: Adds a new location if it doesn't already exist.
- **GetLocationAsync**: Retrieves a location by ID.
- **DeleteLocationAsync**: Deletes a location and its associated weather data.
- **GetAllLocationsAsync**: Retrieves all saved locations.
- **GetWeatherByLocationIdAsync**: Fetches weather forecast data for a saved location.

## Configuration

1. **Database**: Configure `ApplicationDbContext` in `appsettings.json` with the necessary database connection string.
2. **Open-Meteo API**: The `WeatherService` fetches weather data from Open-Meteo. Configure the base URL as needed.

## Setup

1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd ForecastApp

2. Restore Dependencies:
   ```bash
   dotnet restore

3. Run Migrations (for EF Core):
   ```bash
   dotnet ef database update

4. Run the Application:
   ```bash
   dotnet run

## Technologies used

- **ASP.NET Core** for API handling
- **Entity Framework Core** for database management
- **Open-Meteo API** for weather data
- **SQL Database** for data storage

## Error Handling

- **`404 Not Found`**: Returned when no data is available for the requested resource.
- **`400 Bad Request`**: Returned for invalid requests or when the requested action could not be completed.

