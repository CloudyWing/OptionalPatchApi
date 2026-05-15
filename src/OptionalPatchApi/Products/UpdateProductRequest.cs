using System.ComponentModel.DataAnnotations;
using CloudyWing.OptionalPatchApi.OptionalValues;

namespace CloudyWing.OptionalPatchApi.Products;

/// <summary>
/// Represents fields accepted by the product PATCH endpoints.
/// </summary>
public sealed class UpdateProductRequest {
    /// <summary>
    /// Gets or sets the optional product name update.
    /// </summary>
    [Required]
    [StringLength(80, MinimumLength = 1)]
    public OptionalValue<string> Name { get; set; }

    /// <summary>
    /// Gets or sets the optional product description update.
    /// </summary>
    [StringLength(200)]
    public OptionalValue<string?> Description { get; set; }

    /// <summary>
    /// Gets or sets the optional product price update.
    /// </summary>
    [Range(typeof(decimal), "0.01", "999999")]
    public OptionalValue<decimal> Price { get; set; }

    /// <summary>
    /// Gets or sets the optional discontinued date update.
    /// </summary>
    public OptionalValue<DateOnly?> DiscontinuedOn { get; set; }
}
