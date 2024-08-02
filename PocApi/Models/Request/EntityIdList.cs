using System.Text.Json.Serialization;

namespace PocApi.Models.Request;

public class EntityIdList
{
    [JsonPropertyName("entities_id")]
    public required IEnumerable<string> EntitiesId { get; set; }
}
