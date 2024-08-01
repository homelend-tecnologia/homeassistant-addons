using System.Text.Json.Serialization;

namespace PocApi.Models.HomeAssistant.State;

public class Context
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }
}
