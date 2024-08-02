using System.Text.Json.Serialization;

namespace PocApi.Models.Request;

public class Entity
{
    [JsonPropertyName("entity_id")]
    public string? EntityId { get; set; }
}
