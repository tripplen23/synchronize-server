using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IProductService
    {
        Task<IEnumerable<ProductReadDto>> GetAllProductsAsync(ProductQueryOptions? options);
        Task<IEnumerable<ProductReadDto>> GetProductsByCategoryAsync(Guid categoryId);
        Task<ProductReadDto> GetProductByIdAsync(Guid productId);
        Task<ProductReadDto> CreateProductAsync(ProductCreateDto productCreateDto);
        Task<ProductReadDto> UpdateProductByIdAsync(Guid id, ProductUpdateDto productUpdateDto);
        Task<bool> DeleteProductByIdAsync(Guid productId);
    }
}