using System.Text.Json;
using System.Text.Json.Serialization;

namespace CloudyWing.OptionalPatchApi.OptionalValues;

/// <summary>
/// Creates JSON converters for <see cref="OptionalValue{T}"/>.
/// </summary>
public sealed class OptionalValueJsonConverterFactory : JsonConverterFactory {
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) {
        return OptionalValueType.TryGetValueType(typeToConvert, out Type _);
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
        OptionalValueType.TryGetValueType(typeToConvert, out Type valueType);
        Type converterType = typeof(OptionalValueJsonConverter<>).MakeGenericType(valueType);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
