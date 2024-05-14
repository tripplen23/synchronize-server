using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface ICartItemService
    {
        Task<CartReadDto> UpdateCartItemQuantityAsync(Guid cartId, CartUpdateDto cartUpdateDto);
    }
}