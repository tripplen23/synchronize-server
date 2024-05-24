using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Service.src.DTO
{
    public class OrderReadDto : BaseEntity
    {
        public UserReadDto User { get; set; }
        public IEnumerable<OrderProductReadDto> OrderProducts { get; set; }
        public ShippingInfoReadDto ShippingInfo { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    public class OrderCreateDto
    {
        public IEnumerable<OrderProductCreateDto> OrderProducts { get; set; }
        public ShippingInfoCreateDto ShippingInfo { get; set; }
        public OrderStatus? OrderStatus { get; set; }
    }

    public class OrderUpdateStatusDto
    {
        public Guid OrderId { get; set; }
        public ShippingInfoUpdateDto? ShippingInfo { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    public class OrderReadUpdateDto : BaseEntity
    {
        public Guid UserId { get; set; }
        public UserReadDto User { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public IEnumerable<OrderProductReadDto> OrderProducts { get; set; }
        public ShippingInfoReadDto ShippingInfo { get; set; }
        public decimal TotalPrice { get; set; }
    }
}