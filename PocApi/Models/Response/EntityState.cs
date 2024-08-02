using System.Text.Json.Serialization;

namespace PocApi.Models.Response;

public class EntityState
{
    [JsonPropertyName("entity_id")]
    public string? EntityId { get; set; }
    [JsonPropertyName("state")]
    public string? State { get; set; }
}
