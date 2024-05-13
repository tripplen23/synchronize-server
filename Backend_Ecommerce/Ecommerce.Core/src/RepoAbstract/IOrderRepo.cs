using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IOrderRepo
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(BaseQueryOptions options);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<Order> CreateOrderAsync(Order createdOrder);
        Task<Order> UpdateOrderAsync(Order updatedOrder);
        Task<bool> DeleteOrderByIdAsync(Guid orderId);
    }
}