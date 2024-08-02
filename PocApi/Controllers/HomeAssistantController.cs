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
}
