using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IProductRepo
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(ProductQueryOptions? options);
        Task<IEnumerable<Product>> GetMostPurchasedProductsAsync(int topNumber);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<Product> GetProductByIdAsync(Guid productId);
        Task<Product> CreateProductAsync(Product newProduct);
        Task<Product> UpdateProductByIdAsync(Product updatedProduct);
        Task<bool> DeleteProductByIdAsync(Guid productId);
    }
}