using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CloudyWing.OptionalPatchApi.OptionalValues;

/// <summary>
/// Provides model binders for <see cref="OptionalValue{T}"/>.
/// </summary>
public sealed class OptionalValueModelBinderProvider : IModelBinderProvider {
    /// <inheritdoc/>
    public IModelBinder? GetBinder(ModelBinderProviderContext context) {
        ArgumentNullException.ThrowIfNull(context);

        if (!OptionalValueType.TryGetValueType(context.Metadata.ModelType, out Type valueType)) {
            return null;
        }

        Type binderType = typeof(OptionalValueModelBinder<>).MakeGenericType(valueType);
        return (IModelBinder)Activator.CreateInstance(binderType)!;
    }
}
