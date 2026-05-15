namespace CloudyWing.OptionalPatchApi.Products;

/// <summary>
/// Represents a product stored by the sample API.
/// </summary>
public sealed class Product {
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the optional product description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the date when the product becomes unavailable.
    /// </summary>
    public DateOnly? DiscontinuedOn { get; set; }
}
