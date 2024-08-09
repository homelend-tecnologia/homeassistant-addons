using System.Text.Json.Serialization;

namespace PocApi.Models.HomeAssistant.History;

public class SensorData
{
    [JsonPropertyName("entity_id")]
    public string? EntityId { get; set; }
    [JsonPropertyName("state")]
    public string? State { get; set; }
    [JsonPropertyName("last_changed")]
    public DateTimeOffset? LastChanged { get; set; }
    [JsonPropertyName("last_reported")]
    public DateTimeOffset? LastReported { get; set; }
    [JsonPropertyName("last_updated")]
    public DateTimeOffset? LastUpdated { get; set; }
}
