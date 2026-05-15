using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CloudyWing.OptionalPatchApi.OptionalValues;

/// <summary>
/// Applies a validation attribute to the supplied optional value.
/// </summary>
public sealed class OptionalValueModelValidator : IModelValidator {
    private readonly ValidationAttribute attribute;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalValueModelValidator"/> class.
    /// </summary>
    /// <param name="attribute">The attribute to apply to the wrapped value.</param>
    public OptionalValueModelValidator(ValidationAttribute attribute) {
        this.attribute = attribute;
    }

    /// <inheritdoc/>
    public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context) {
        if (context.Model is not IOptionalValue optionalValue || !optionalValue.HasValue) {
            yield break;
        }

        string displayName = context.ModelMetadata.GetDisplayName();
        ValidationContext validationContext = new(context.Model!) {
            DisplayName = displayName,
            MemberName = context.ModelMetadata.PropertyName
        };
        ValidationResult? validationResult = attribute.GetValidationResult(optionalValue.UntypedValue, validationContext);

        if (validationResult != ValidationResult.Success) {
            yield return new ModelValidationResult("", validationResult?.ErrorMessage ?? attribute.FormatErrorMessage(displayName));
        }
    }
}
