using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PocApi.Models.HomeAssistant.State;

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

    public HomeAssistantController(ILogger<HomeAssistantController> logger,
        IOptions<JsonOptions> jsonOptions,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient("homeassistant");

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

}
