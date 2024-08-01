using Microsoft.AspNetCore.Mvc;

namespace PocApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HomeAssistantController : ControllerBase
{

    private readonly ILogger<HomeAssistantController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    public HomeAssistantController(ILogger<HomeAssistantController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient("homeassistant");

        _logger.LogInformation("HomeAssistantController created");
    }

    [HttpGet(Name = "Healthy")]
    public IResult Get()
    {
        return Results.Json("Home Assistant");
    }
}
