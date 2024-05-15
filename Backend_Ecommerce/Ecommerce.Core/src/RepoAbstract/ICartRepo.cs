using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface ICartRepo
    {
        Task<IEnumerable<Cart>> GetAllCartsAsync(BaseQueryOptions options);
        Task<Cart> GetCartByUserIdAsync(Guid userId);
        Task<Cart> GetCartByIdAsync(Guid cartId);
        Task<Cart> CreateCartAsync(Cart createdCart);
        Task<Cart> UpdateCartAsync(Cart updatedCart);
        Task<bool> DeleteCartByIdAsync(Guid cartId);
    }
}