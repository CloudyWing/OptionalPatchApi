namespace CloudyWing.OptionalPatchApi.OptionalValues;

/// <summary>
/// Provides reflection helpers for <see cref="OptionalValue{T}"/>.
/// </summary>
public static class OptionalValueType {
    /// <summary>
    /// Determines whether the specified type is <see cref="OptionalValue{T}"/>.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="valueType">The wrapped value type.</param>
    /// <returns><see langword="true"/> when the type is an optional value.</returns>
    public static bool TryGetValueType(Type type, out Type valueType) {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(OptionalValue<>)) {
            valueType = type.GetGenericArguments()[0];
            return true;
        }

        valueType = typeof(void);
        return false;
    }
}
