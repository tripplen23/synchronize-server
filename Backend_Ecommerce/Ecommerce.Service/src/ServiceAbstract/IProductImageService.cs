using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageReadDto>> GetProductImagesByProductIdAsync(Guid productId);
        Task UpdateProductImagesAsync(Guid productId, IEnumerable<ProductImageUpdateDto> imagesToUpdate);
    }
}