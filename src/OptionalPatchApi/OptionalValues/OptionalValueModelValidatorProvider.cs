using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CloudyWing.OptionalPatchApi.OptionalValues;

/// <summary>
/// Wraps validation attributes applied to <see cref="OptionalValue{T}"/> properties.
/// </summary>
public sealed class OptionalValueModelValidatorProvider : IModelValidatorProvider {
    /// <inheritdoc/>
    public void CreateValidators(ModelValidatorProviderContext context) {
        ArgumentNullException.ThrowIfNull(context);

        if (!OptionalValueType.TryGetValueType(context.ModelMetadata.ModelType, out Type _)) {
            return;
        }

        foreach (ValidatorItem validatorItem in context.Results) {
            if (validatorItem.ValidatorMetadata is ValidationAttribute attribute) {
                validatorItem.Validator = new OptionalValueModelValidator(attribute);
                validatorItem.IsReusable = true;
            }
        }
    }
}
