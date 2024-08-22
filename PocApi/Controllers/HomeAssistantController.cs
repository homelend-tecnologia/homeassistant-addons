using InfluxDB.Client;
using InfluxDB.Client.Flux;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PocApi.Models.HomeAssistant.History;
using PocApi.Models.HomeAssistant.State;

using System.Globalization;
using System.Text;
using System.Text.Json;

namespace PocApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeAssistantController : ControllerBase
{

    private readonly ILogger<HomeAssistantController> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly IFluxClient _fluxClient;

    public HomeAssistantController(ILogger<HomeAssistantController> logger,
        IOptions<JsonOptions> jsonOptions,
        IHttpClientFactory httpClientFactory,
        IFluxClient fluxClient)
    {
        _logger = logger;
        _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient("homeassistant");
        _fluxClient = fluxClient;

        _logger.LogInformation("HomeAssistantController created");
    }

    [HttpGet]
    [Route("device/states")]
    public async Task<IResult> GetDeviceStatesAsync()
    {
        var response = await _httpClient.GetAsync("states");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Failed to get states from Home Assistant", statusCode: (int)response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync();
        var states = JsonSerializer.Deserialize<List<StateObject>>(content);

        return Results.Json(states, _jsonOptions);
    }

    [HttpGet]
    [Route("device/{deviceId}/state")]
    public async Task<IResult> GetDeviceStateAsync(string deviceId)
    {
        var response = await _httpClient.GetAsync($"states/{deviceId}");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Failed to get device state from Home Assistant", statusCode: (int)response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync();
        var state = JsonSerializer.Deserialize<StateObject>(content);

        return Results.Json(state, _jsonOptions);
    }

    [HttpGet]
    [Route("device/light/{deviceId}/state")]
    public async Task<IResult> GetLightStateAsync(string deviceId)
    {
        var response = await _httpClient.GetAsync($"states/light.{deviceId}");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Failed to get light state from Home Assistant", statusCode: (int)response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync();
        var state = JsonSerializer.Deserialize<StateObject>(content);

        var retState = new Models.Response.EntityState
        {
            EntityId = deviceId,
            State = state?.State switch
            {
                "on" or "off" => state.State,
                _ => "unknown"
            }
        };

        return Results.Json(retState, _jsonOptions);
    }

    [HttpPost]
    [Route("device/light/states")]
    public async Task<IResult> GetLightStatesAsync(Models.Request.EntityIdList entities)
    {
        var lightStates = new List<Models.Response.EntityState>();

        foreach (var entity in entities.EntitiesId)
        {
            var response = await GetLightStateAsync(entity);

            if (response is JsonHttpResult<Models.Response.EntityState> jsonResult)
            {
                if (jsonResult.Value is Models.Response.EntityState lightState)
                {
                    lightStates.Add(lightState);
                }
            }
        }

        return Results.Json(lightStates, _jsonOptions);
    }

    [HttpGet]
    [Route("device/sensor/{deviceId}/state")]
    public async Task<IResult> GetSensorStateAsync(string deviceId)
    {
        var response = await _httpClient.GetAsync($"states/sensor.{deviceId}");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Failed to get sensor state from Home Assistant", statusCode: (int)response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync();
        var state = JsonSerializer.Deserialize<StateObject>(content);

        var retState = new Models.Response.EntityState
        {
            EntityId = deviceId,
            State = state?.State ?? "unknown"
        };

        return Results.Json(retState, _jsonOptions);
    }

    [HttpPost]
    [Route("device/sensor/states")]
    public async Task<IResult> GetSensorStatesAsync(Models.Request.EntityIdList entities)
    {
        var sensorStates = new List<Models.Response.EntityState>();

        foreach (var entity in entities.EntitiesId)
        {
            var response = await GetSensorStateAsync(entity);

            if (response is JsonHttpResult<Models.Response.EntityState> jsonResult)
            {
                if (jsonResult.Value is Models.Response.EntityState sensorState)
                {
                    sensorStates.Add(sensorState);
                }
            }
        }

        return Results.Json(sensorStates, _jsonOptions);
    }

    [HttpGet]
    [Route("device/binary_sensor/{deviceId}/state")]
    public async Task<IResult> GetBinarySensorStateAsync(string deviceId)
    {
        var response = await _httpClient.GetAsync($"states/binary_sensor.{deviceId}");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Failed to get sensor state from Home Assistant", statusCode: (int)response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync();
        var state = JsonSerializer.Deserialize<StateObject>(content);

        var retState = new Models.Response.EntityState
        {
            EntityId = deviceId,
            State = state?.State ?? "unknown"
        };

        return Results.Json(retState, _jsonOptions);
    }

    [HttpPost]
    [Route("device/binary_sensor/states")]
    public async Task<IResult> GetBinarySensorStatesAsync(Models.Request.EntityIdList entities)
    {
        var sensorStates = new List<Models.Response.EntityState>();

        foreach (var entity in entities.EntitiesId)
        {
            var response = await GetBinarySensorStateAsync(entity);

            if (response is JsonHttpResult<Models.Response.EntityState> jsonResult)
            {
                if (jsonResult.Value is Models.Response.EntityState sensorState)
                {
                    sensorStates.Add(sensorState);
                }
            }
        }

        return Results.Json(sensorStates, _jsonOptions);
    }

    [HttpPost]
    [Route("device/light/{deviceId}/turn_{newState}")]
    public async Task<IResult> TurnLightAsync(string deviceId, string newState)
    {
        var response = await _httpClient.PostAsync($"services/light/turn_{newState}", new StringContent($"{{\"entity_id\": \"light.{deviceId}\"}}"));

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Failed to turn light on/off", statusCode: (int)response.StatusCode);
        }

        return Results.Ok();
    }

    [HttpGet]
    [Route("device/sensor/{deviceId}/history")]
    public async Task<IResult> GetSensorStateHistoryAsync(string deviceId)
    {
        var response = await _httpClient.GetAsync($"history/period?no_attributes=&minimal_response&significant_changes_only&filter_entity_id=sensor.{deviceId}");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Failed to get sensor state history from Home Assistant", statusCode: (int)response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync();
        var stateGroupList = JsonSerializer.Deserialize<List<List<SensorData>>>(content);

        var retStateList = stateGroupList?.FirstOrDefault();

        return Results.Json(retStateList, _jsonOptions);
    }

    [HttpGet]
    [Route("device/energy-consumption/history")]
    public async Task<IResult> GetEnergyConsumptionHistoryAsync(
        [FromQuery(Name = "device")] string deviceId = "consumo_de_energia_total",
        [FromQuery(Name = "timeZoneOffset")] TimeSpan? timespan = null,
        [FromQuery(Name = "groupby")] string groupBy = "hour",
        [FromQuery(Name = "start")] DateTimeOffset? startDateTime = null,
        [FromQuery(Name = "end")] DateTimeOffset? endDateTime = null
        )
    {
        TimeSpan timeZoneOffset = timespan ?? TimeSpan.Zero;
        groupBy = groupBy.Trim().ToLowerInvariant();

        var startTime = ObterMeiaNoiteLocalEmUTC(timeZoneOffset, startDateTime);
        var endTime = endDateTime?.ToUniversalTime() ?? DateTimeOffset.UtcNow;

        var response = await _httpClient.GetAsync($"history/period/{startTime:O}?no_attributes=&minimal_response&significant_changes_only&end_time={endTime:u}&filter_entity_id=sensor.{deviceId}");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Failed to get energy consumption history from Home Assistant", statusCode: (int)response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync();
        var stateGroupList = JsonSerializer.Deserialize<List<List<SensorData>>>(content);

        var deviceStateList = stateGroupList?.FirstOrDefault();

        decimal? lastValue = null;

        var groupedStates = deviceStateList?
            .GroupBy(c => groupBy switch
                    {
                        "minute" => new { c.LastChanged?.Year, c.LastChanged?.Month, c.LastChanged?.Day, c.LastChanged?.Hour, c.LastChanged?.Minute },
                        "hour" => new { c.LastChanged?.Year, c.LastChanged?.Month, c.LastChanged?.Day, c.LastChanged?.Hour },
                        "day" => new { c.LastChanged?.Year, c.LastChanged?.Month, c.LastChanged?.Day },
                        "month" => new { c.LastChanged?.Year, c.LastChanged?.Month },
                        "year" => new { c.LastChanged?.Year },
                        _ => (new { c.LastChanged?.Year, c.LastChanged?.Month, c.LastChanged?.Day, c.LastChanged?.Hour }) as object,
                    })
            .Select(g =>
            {
                dynamic key = g.Key;
                var periodStart = (groupBy switch
                {
                    "minute" => new DateTimeOffset(key.Year, key.Month, key.Day, key.Hour, key.Minute, 0, TimeSpan.Zero),
                    "hour" => new DateTimeOffset(key.Year, key.Month, key.Day, key.Hour, 0, 0, TimeSpan.Zero),
                    "day" => new DateTimeOffset(key.Year, key.Month, key.Day, 0, 0, 0, TimeSpan.Zero),
                    "month" => new DateTimeOffset(key.Year, key.Month, 1, 0, 0, 0, TimeSpan.Zero),
                    "year" => new DateTimeOffset(key.Year, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    _ => new DateTimeOffset(key.Year, key.Month, key.Day, key.Hour, 0, 0, TimeSpan.Zero),
                }).ToUniversalTime();


                var firstValue = lastValue ?? decimal.Parse(g.First().State ?? string.Empty, CultureInfo.InvariantCulture);
                var lastValueInGroup = decimal.Parse(g.Last().State ?? string.Empty, CultureInfo.InvariantCulture);

                // Atualiza o lastValue para o próximo grupo
                lastValue = lastValueInGroup;

                return new
                {
                    Period = periodStart,
                    TotalConsumption = Math.Round(lastValueInGroup - firstValue, 2)
                };
            }).ToList();




        return Results.Json(groupedStates, _jsonOptions);
    }

    private static DateTimeOffset ObterMeiaNoiteLocalEmUTC(TimeSpan timeZoneOffset, DateTimeOffset? dateTime = null)
    {
        var utcNow = dateTime?.UtcDateTime ?? DateTimeOffset.UtcNow;
        var localDateTime = utcNow.Add(timeZoneOffset);
        var midnightLocal = new DateTimeOffset(localDateTime.Year, localDateTime.Month, localDateTime.Day, 0, 0, 0, timeZoneOffset);

        var midnightUTC = midnightLocal.ToUniversalTime();

        return midnightUTC;
    }

    [HttpGet]
    [Route("influx")]
    public async Task<IResult> GetInfluxDataAsync()
    {

        string query = @"
            base_query = from(bucket: ""ha"")
              |> range(start: -24h)
              |> filter(fn: (r) => r[""_measurement""] == ""°C"")
              |> filter(fn: (r) => r[""_field""] == ""value"")
              |> filter(fn: (r) => r[""entity_id""] == ""temperatura_media_interna"")
            tmin = base_query 
              |> aggregateWindow(every:  1h, fn: min, createEmpty: false)
              |> keep(columns: [""_time"", ""_value""]) 
              |> movingAverage(n: 3)
              |> rename(columns: {_value: ""min""})
            tmax = base_query 
              |> aggregateWindow(every: 1h, fn: max, createEmpty: false)
              |> keep(columns: [""_time"", ""_value""]) 
              |> movingAverage(n: 3)
              |> rename(columns: {_value: ""max""})
            tmean = base_query 
              |> aggregateWindow(every: 1h, fn: mean, createEmpty: false)
              |> keep(columns: [""_time"", ""_value""]) 
              |> movingAverage(n: 3)
              |> rename(columns: {_value: ""mean""})

            tminmax = join(tables: {tmin: tmin, tmax: tmax}, on: [""_time""])
            join(tables: {tminmax: tminmax, tmean: tmean}, on: [""_time""])
            ";

        var tables = await _fluxClient.QueryAsync(query);

        // Lista para armazenar os resultados formatados
        var formattedResults = new List<dynamic>();

        foreach (var table in tables)
        {
            foreach (var row in table.Records)
            {
                formattedResults.Add(new
                {
                    time = row.GetTime().ToString(),
                    min = row.GetValueByKey("min"),
                    max = row.GetValueByKey("max"),
                    mean = row.GetValueByKey("mean")
                });
            }
        }

        return Results.Ok(formattedResults);
    }

}
