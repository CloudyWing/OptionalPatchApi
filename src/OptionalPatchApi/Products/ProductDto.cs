namespace CloudyWing.OptionalPatchApi.Products;

/// <summary>
/// Represents product data returned by the API.
/// </summary>
public sealed class ProductDto {
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the optional product description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public required decimal Price { get; init; }

    /// <summary>
    /// Gets or sets the date when the product becomes unavailable.
    /// </summary>
    public DateOnly? DiscontinuedOn { get; init; }

    /// <summary>
    /// Creates a DTO from a product entity.
    /// </summary>
    /// <param name="product">The product entity.</param>
    /// <returns>The product DTO.</returns>
    public static ProductDto FromProduct(Product product) {
        return new ProductDto {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DiscontinuedOn = product.DiscontinuedOn
        };
    }
}
