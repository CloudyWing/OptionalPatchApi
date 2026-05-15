using System.Text.Json;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CloudyWing.OptionalPatchApi.OpenApi;

/// <summary>
/// Normalizes form schemas generated for optional values.
/// </summary>
public sealed class OptionalValueOperationTransformer : IOpenApiOperationTransformer {
    private static readonly string[] FormContentTypes = [
        "application/x-www-form-urlencoded",
        "multipart/form-data"
    ];

    /// <inheritdoc/>
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken
    ) {
        if (operation.RequestBody?.Content is null) {
            return Task.CompletedTask;
        }

        foreach (string contentType in FormContentTypes) {
            if (operation.RequestBody.Content.TryGetValue(contentType, out OpenApiMediaType? mediaType)
                && mediaType.Schema is OpenApiSchema schema
            ) {
                mediaType.Encoding ??= new Dictionary<string, OpenApiEncoding>();
                NormalizeFormSchema(schema, mediaType.Encoding);
                NormalizePropertyNames(schema, mediaType.Encoding);
            }
        }

        return Task.CompletedTask;
    }

    private static void NormalizeFormSchema(
        OpenApiSchema schema,
        IDictionary<string, OpenApiEncoding> encoding
    ) {
        if (schema.Properties is null) {
            return;
        }

        List<string> keys = [.. schema.Properties.Keys];

        foreach (string key in keys) {
            if (key.EndsWith(".HasValue", StringComparison.OrdinalIgnoreCase)) {
                schema.Properties.Remove(key);
                encoding.Remove(key);
                continue;
            }

            if (!key.EndsWith(".Value", StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            string normalizedKey = key[..^".Value".Length];
            if (schema.Properties.Remove(key, out IOpenApiSchema? propertySchema)) {
                schema.Properties[normalizedKey] = propertySchema;
            }

            if (encoding.Remove(key, out OpenApiEncoding? propertyEncoding)) {
                encoding[normalizedKey] = propertyEncoding;
            }
        }
    }

    private static void NormalizePropertyNames(
        OpenApiSchema schema,
        IDictionary<string, OpenApiEncoding> encoding
    ) {
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

            if (encoding.Remove(key, out OpenApiEncoding? propertyEncoding)) {
                encoding[normalizedKey] = propertyEncoding;
            }
        }
    }
}
