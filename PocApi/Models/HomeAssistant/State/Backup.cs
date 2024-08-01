using System.Text.Json.Serialization;

namespace PocApi.Models.HomeAssistant.State;

public class Backup
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }

    [JsonPropertyName("state")]

    public string? State { get; set; }

    [JsonPropertyName("size")]
    public string? Size { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }
}
