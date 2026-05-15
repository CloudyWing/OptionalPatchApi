using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CloudyWing.OptionalPatchApi.OptionalValues;

/// <summary>
/// Represents a field that can distinguish between omitted and supplied values.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public readonly record struct OptionalValue<T> : IOptionalValue {
    private readonly T value;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalValue{T}"/> struct.
    /// </summary>
    /// <param name="value">The supplied value.</param>
    public OptionalValue(T value) {
        HasValue = true;
        this.value = value;
    }

    /// <summary>
    /// Gets an omitted optional value.
    /// </summary>
    /// <returns>An optional value without a supplied value.</returns>
    public static OptionalValue<T> Empty() => new();

    /// <summary>
    /// Gets a value indicating whether the client supplied the field.
    /// </summary>
    [ValidateNever]
    public bool HasValue { get; }

    /// <summary>
    /// Gets the supplied value.
    /// </summary>
    /// <exception cref="InvalidOperationException">The field was not supplied.</exception>
    [ValidateNever]
    public T Value => HasValue
        ? value
        : throw new InvalidOperationException("Optional value was not supplied.");

    object? IOptionalValue.UntypedValue => HasValue ? value : null;

    /// <summary>
    /// Converts a supplied value to an optional value.
    /// </summary>
    /// <param name="value">The supplied value.</param>
    public static implicit operator OptionalValue<T>(T value) => new(value);
}
