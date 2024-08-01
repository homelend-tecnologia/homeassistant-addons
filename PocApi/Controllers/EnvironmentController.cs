using Microsoft.AspNetCore.Mvc;

using System.Collections;

namespace PocApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnvironmentController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEnvironmentVariables()
    {
        var environmentVariables = Environment.GetEnvironmentVariables();
        var result = new Dictionary<string, string>();

        foreach (DictionaryEntry entry in environmentVariables)
        {
            result.Add(entry.Key.ToString() ?? string.Empty, entry.Value?.ToString() ?? string.Empty);
        }

        return Ok(result);
    }
}
