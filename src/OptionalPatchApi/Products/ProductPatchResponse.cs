namespace CloudyWing.OptionalPatchApi.Products;

/// <summary>
/// Represents the result of applying a product patch.
/// </summary>
public sealed class ProductPatchResponse {
    /// <summary>
    /// Gets or sets the updated product.
    /// </summary>
    public required ProductDto Product { get; init; }

    /// <summary>
    /// Gets or sets the names of fields supplied by the client.
    /// </summary>
    public required IReadOnlyList<string> ChangedFields { get; init; } = [];
}
