using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sofra.Api.Helper;

/// <summary>
/// Serializes every <see cref="DateTime"/> as an ISO-8601 UTC string with a
/// trailing "Z" (truncated round-trip "o" format), matching the frontend
/// contract. EF Core reads <c>datetime2</c> values back with
/// <c>DateTimeKind.Unspecified</c>, which the default serializer writes
/// without the offset; the values are always stored in UTC.
/// </summary>
public sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetDateTime();

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Local => value.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => value
        };

        writer.WriteStringValue(utc);
    }
}
