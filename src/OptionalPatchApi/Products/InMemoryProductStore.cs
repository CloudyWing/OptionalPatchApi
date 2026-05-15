using System.Collections.Concurrent;

namespace CloudyWing.OptionalPatchApi.Products;

/// <summary>
/// Stores sample products in memory.
/// </summary>
public sealed class InMemoryProductStore : IProductStore {
    private readonly ConcurrentDictionary<int, Product> products = new([
        new KeyValuePair<int, Product>(
            1,
            new Product {
                Id = 1,
                Name = "Notebook Pro",
                Description = "Initial description",
                Price = 42000m,
                DiscontinuedOn = null
            }
        )
    ]);

    /// <inheritdoc/>
    public Product? Find(int id) {
        return products.TryGetValue(id, out Product? product) ? product : null;
    }

    /// <inheritdoc/>
    public ProductPatchResponse? Patch(int id, UpdateProductRequest request) {
        Product? product = Find(id);
        if (product is null) {
            return null;
        }

        List<string> changedFields = [];
        ApplyPatch(product, request, changedFields);

        return new ProductPatchResponse {
            Product = ProductDto.FromProduct(product),
            ChangedFields = changedFields
        };
    }

    private static void ApplyPatch(Product product, UpdateProductRequest request, ICollection<string> changedFields) {
        if (request.Name.HasValue) {
            product.Name = request.Name.Value;
            changedFields.Add(nameof(request.Name));
        }

        if (request.Description.HasValue) {
            product.Description = request.Description.Value;
            changedFields.Add(nameof(request.Description));
        }

        if (request.Price.HasValue) {
            product.Price = request.Price.Value;
            changedFields.Add(nameof(request.Price));
        }

        if (request.DiscontinuedOn.HasValue) {
            product.DiscontinuedOn = request.DiscontinuedOn.Value;
            changedFields.Add(nameof(request.DiscontinuedOn));
        }
    }
}
