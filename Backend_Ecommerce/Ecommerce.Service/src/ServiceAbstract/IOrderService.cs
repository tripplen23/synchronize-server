using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync(BaseQueryOptions options);
        Task<IEnumerable<OrderReadDto>> GetOrdersByUserIdAsync(Guid userId);
        Task<OrderReadDto> GetOrderByIdAsync(Guid orderId);
        Task<OrderReadDto> CreateOrderAsync(Guid userId, OrderCreateDto orderCreateDto);
        Task<OrderReadUpdateDto> UpdateOrderStatusAsync(Guid orderId, OrderUpdateStatusDto orderUpdateStatusDto);
        Task<bool> DeleteOrderByIdAsync(Guid orderId);
    }
}