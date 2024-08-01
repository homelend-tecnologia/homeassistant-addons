using System.Text.Json;
using System.Text.Json.Serialization;

namespace PocApi.Converters.Json;

public class StringAllowAllValuesConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.True => "true",
            JsonTokenType.False => "false",
            JsonTokenType.Number => reader.GetDouble().ToString(),
            JsonTokenType.Null => "null",
            _ => throw new JsonException($"Unexpected token parsin string. Token: {reader.TokenType}")
        };
        
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
