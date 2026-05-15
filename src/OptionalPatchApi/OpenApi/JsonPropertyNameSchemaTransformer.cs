using System.Text.Json;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CloudyWing.OptionalPatchApi.OpenApi;

/// <summary>
/// Normalizes OpenAPI schema property names to match JSON serialization names.
/// </summary>
public sealed class JsonPropertyNameSchemaTransformer : IOpenApiSchemaTransformer {
    /// <inheritdoc/>
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken
    ) {
        NormalizePropertyNames(schema);
        return Task.CompletedTask;
    }

    private static void NormalizePropertyNames(OpenApiSchema schema) {
        if (schema.Properties is null) {
            return;
        }

        List<string> keys = [.. schema.Properties.Keys];

        foreach (string key in keys) {
            string normalizedKey = JsonNamingPolicy.CamelCase.ConvertName(key);
            if (key == normalizedKey) {
                continue;
            }

            if (schema.Properties.Remove(key, out IOpenApiSchema? propertySchema)) {
                schema.Properties[normalizedKey] = propertySchema;
            }
        }

        if (schema.Required is null || schema.Required.Count == 0) {
            return;
        }

        HashSet<string> required = new(StringComparer.Ordinal);
        foreach (string key in schema.Required) {
            required.Add(JsonNamingPolicy.CamelCase.ConvertName(key));
        }

        schema.Required = required;
    }
}
