using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface ICartService
    {
        Task<IEnumerable<CartReadDto>> GetAllCartsAsync(BaseQueryOptions options);
        // Get Cart by user Id
        Task<CartReadDto> GetCartByUserIdAsync(Guid userId);
        Task<CartReadDto> GetCartByIdAsync(Guid cartId);
        Task<CartReadDto> CreateCartAsync(Guid userId, CartCreateDto cartCreateDto);
        Task<bool> DeleteCartByIdAsync(Guid cartId);

    }
}