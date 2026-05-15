namespace CloudyWing.OptionalPatchApi.OptionalValues;

/// <summary>
/// Exposes non-generic metadata for an optional value.
/// </summary>
public interface IOptionalValue {
    /// <summary>
    /// Gets a value indicating whether the client supplied the field.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Gets the supplied value as an object.
    /// </summary>
    public object? UntypedValue { get; }
}
