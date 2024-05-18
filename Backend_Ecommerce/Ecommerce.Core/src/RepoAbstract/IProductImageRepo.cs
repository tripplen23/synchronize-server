using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IProductImageRepo
    {
        Task<IEnumerable<ProductImage>> GetProductImagesByProductIdAsync(Guid productId);
        Task<ProductImage> GetImageByIdAsync(Guid imageId);
        Task  UpdateImageDataAsync(Guid imageId, string newImageData);
    }
}