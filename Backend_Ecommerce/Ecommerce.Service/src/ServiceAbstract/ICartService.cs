using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface ICartService
    {
        Task<CartReadDto> GetCartByUserIdAsync(Guid userId);
        Task<CartReadDto> CreateCartAsync(Guid userId, CartCreateDto cartCreateDto);
        Task<bool> DeleteCartByIdAsync(Guid cartId);
        Task<bool> ClearCartAsync(Guid cartId);
    }
}