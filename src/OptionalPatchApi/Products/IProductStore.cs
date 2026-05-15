namespace CloudyWing.OptionalPatchApi.Products;

/// <summary>
/// Provides product lookup and patch operations.
/// </summary>
public interface IProductStore {
    /// <summary>
    /// Finds a product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>The product when found; otherwise, <see langword="null"/>.</returns>
    public Product? Find(int id);

    /// <summary>
    /// Applies a partial update to a product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="request">The update request.</param>
    /// <returns>The patch result when the product exists; otherwise, <see langword="null"/>.</returns>
    public ProductPatchResponse? Patch(int id, UpdateProductRequest request);
}
