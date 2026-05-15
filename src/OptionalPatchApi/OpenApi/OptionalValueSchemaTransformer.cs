using CloudyWing.OptionalPatchApi.OptionalValues;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CloudyWing.OptionalPatchApi.OpenApi;

/// <summary>
/// Maps <see cref="OptionalValue{T}"/> schemas to their wrapped value type.
/// </summary>
public sealed class OptionalValueSchemaTransformer : IOpenApiSchemaTransformer {
    /// <inheritdoc/>
    public async Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken
    ) {
        if (!OptionalValueType.TryGetValueType(context.JsonTypeInfo.Type, out Type valueType)) {
            return;
        }

        OpenApiSchema valueSchema = await context.GetOrCreateSchemaAsync(
            valueType, context.ParameterDescription, cancellationToken
        ).ConfigureAwait(false);

        CopySchema(valueSchema, schema);
    }

    private static void CopySchema(OpenApiSchema source, OpenApiSchema target) {
        target.Type = source.Type;
        target.Format = source.Format;
        target.Items = source.Items;
        target.Properties = source.Properties;
        target.Required = source.Required;
        target.Enum = source.Enum;
        target.OneOf = source.OneOf;
        target.AnyOf = source.AnyOf;
        target.AllOf = source.AllOf;
        target.Minimum = source.Minimum;
        target.Maximum = source.Maximum;
        target.MinLength = source.MinLength;
        target.MaxLength = source.MaxLength;
        target.Pattern = source.Pattern;
        target.AdditionalProperties = source.AdditionalProperties;
        target.AdditionalPropertiesAllowed = source.AdditionalPropertiesAllowed;
        target.Description = source.Description;
        target.Title = source.Title;
    }
}
