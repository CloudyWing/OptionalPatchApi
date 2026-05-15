using System.ComponentModel;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CloudyWing.OptionalPatchApi.OptionalValues;

/// <summary>
/// Binds form values into <see cref="OptionalValue{T}"/>.
/// </summary>
/// <typeparam name="T">The wrapped value type.</typeparam>
public sealed class OptionalValueModelBinder<T> : IModelBinder {
    /// <inheritdoc/>
    public Task BindModelAsync(ModelBindingContext bindingContext) {
        ArgumentNullException.ThrowIfNull(bindingContext);

        ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None) {
            bindingContext.Result = ModelBindingResult.Success(OptionalValue<T>.Empty());
            return Task.CompletedTask;
        }

        string? valueText = valueProviderResult.FirstValue;
        Type nullableType = typeof(T);
        Type targetType = Nullable.GetUnderlyingType(nullableType) ?? nullableType;

        if (valueText is null && (Nullable.GetUnderlyingType(nullableType) is not null || !targetType.IsValueType)) {
            bindingContext.Result = ModelBindingResult.Success(new OptionalValue<T>(default!));
            return Task.CompletedTask;
        }

        if (valueText == "" && Nullable.GetUnderlyingType(nullableType) is not null) {
            bindingContext.Result = ModelBindingResult.Success(new OptionalValue<T>(default!));
            return Task.CompletedTask;
        }

        try {
            object? convertedValue = ConvertValue(valueText, targetType);
            bindingContext.Result = ModelBindingResult.Success(new OptionalValue<T>((T)convertedValue!));
        } catch (Exception ex) when (
            ex is ArgumentException
                or FormatException
                or InvalidCastException
                or NotSupportedException
                or OverflowException
        ) {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"The value '{valueText}' is invalid.");
        }

        return Task.CompletedTask;
    }

    private static object? ConvertValue(string? valueText, Type targetType) {
        if (targetType == typeof(string)) {
            return valueText;
        }

        if (valueText is null) {
            throw new FormatException("The supplied value is null.");
        }

        TypeConverter converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(typeof(string))) {
            return converter.ConvertFrom(null, CultureInfo.InvariantCulture, valueText);
        }

        return Convert.ChangeType(valueText, targetType, CultureInfo.InvariantCulture);
    }
}
