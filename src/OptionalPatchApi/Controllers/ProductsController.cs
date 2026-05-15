using CloudyWing.OptionalPatchApi.Products;
using Microsoft.AspNetCore.Mvc;

namespace CloudyWing.OptionalPatchApi.Controllers;

/// <summary>
/// Provides product endpoints for optional PATCH examples.
/// </summary>
[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase {
    private readonly IProductStore productStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController"/> class.
    /// </summary>
    /// <param name="productStore">The product store.</param>
    public ProductsController(IProductStore productStore) {
        this.productStore = productStore;
    }

    /// <summary>
    /// Gets a product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>The product when found.</returns>
    [HttpGet("{id:int}")]
    public ActionResult<ProductDto> Get(int id) {
        Product? product = productStore.Find(id);
        return product is null
            ? NotFound()
            : ProductDto.FromProduct(product);
    }

    /// <summary>
    /// Updates a product from a JSON PATCH-style request body.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="request">The patch request.</param>
    /// <returns>The updated product and changed fields.</returns>
    [HttpPatch("{id:int}/body")]
    public ActionResult<ProductPatchResponse> PatchBody(int id, [FromBody] UpdateProductRequest request) {
        ProductPatchResponse? result = productStore.Patch(id, request);
        return result is null ? NotFound() : result;
    }

    /// <summary>
    /// Updates a product from form fields.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="request">The patch request.</param>
    /// <returns>The updated product and changed fields.</returns>
    [HttpPatch("{id:int}/form")]
    [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
    public ActionResult<ProductPatchResponse> PatchForm(int id, [FromForm] UpdateProductRequest request) {
        ProductPatchResponse? result = productStore.Patch(id, request);
        return result is null ? NotFound() : result;
    }
}
