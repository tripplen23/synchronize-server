using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Service.src.DTO
{
    public class OrderReadDto : BaseEntity
    {
        public UserReadDto User { get; set; }
        public IEnumerable<OrderProductReadDto> OrderProducts { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    }

    public class OrderCreateDto
    {
        public IEnumerable<OrderProductCreateDto> OrderProducts { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    }

    public class OrderUpdateDto
    {
        public Guid OrderId { get; set; }
        public IEnumerable<OrderProductUpdateDto> OrderProducts { get; set; }
    }

    public class OrderUpdateStatusDto
    {
        public Guid OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    public class OrderReadUpdateDto : BaseEntity
    {
        public Guid UserId { get; set; }
        public UserReadDto User { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public IEnumerable<OrderProductReadDto> OrderProducts { get; set; }
    }
}