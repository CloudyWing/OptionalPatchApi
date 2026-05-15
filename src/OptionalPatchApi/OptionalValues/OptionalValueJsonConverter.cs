using System.Text.Json;
using System.Text.Json.Serialization;

namespace CloudyWing.OptionalPatchApi.OptionalValues;

/// <summary>
/// Converts <see cref="OptionalValue{T}"/> as the wrapped JSON value.
/// </summary>
/// <typeparam name="T">The wrapped value type.</typeparam>
public sealed class OptionalValueJsonConverter<T> : JsonConverter<OptionalValue<T>> {
    /// <inheritdoc/>
    public override OptionalValue<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        T? value = JsonSerializer.Deserialize<T>(ref reader, options);

        if (value is null && typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) is null) {
            throw new JsonException($"Null value is not allowed for non-nullable type {typeof(T)}.");
        }

        return new OptionalValue<T>(value!);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, OptionalValue<T> value, JsonSerializerOptions options) {
        if (value.HasValue) {
            JsonSerializer.Serialize(writer, value.Value, options);
            return;
        }

        writer.WriteNullValue();
    }
}
