## .Net Core Wheather API App Demo

This demo app was built using Visual Studio 2022 and .Net Core 8.0 on windows. The git commits are organized in logical order so it should be easy enough to follow the progression of the project.

### Building and Testing using Visual Studio 2022:

1. Open the solution file
2. Press the green `>` debug button to bulid and start the app in chosen environment (Docker is supported)

This will open the new browser tab/window with Swagger UI loaded and ready to test the endpoint

### Building and Testing using Docker

##### On Windows:
1. Install docker for windows
2. Switch it to Linux containers (using the tray icon menu)

#### Windows and Linux
3. Run the following commands

```
cd <project_checkout_dir>
docker build --force-rm -t weatherapi .
docker run -dt -e "ASPNETCORE_ENVIRONMENT=Development" -p 8080:8080 --name WeatherApi weatherapi 
```

Head to
* http://localhost:8080/swagger to test using Swagger UI
* http://localhost:8080/currentWeather/{latitude},{longitude} - to test the endpoint directly