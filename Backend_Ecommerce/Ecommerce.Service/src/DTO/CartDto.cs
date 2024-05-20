using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class CartReadDto : BaseEntity
    {
        public Guid UserId { get; set; }
        public UserReadDto User { get; set; }
        public IEnumerable<CartItemReadDto> CartItems { get; set; }
    }

    public class CartCreateDto
    {
        public IEnumerable<CartItemCreateDto> CartItems { get; set; }
    }

    public class CartUpdateDto
    {
        public Guid CartId { get; set; }
        public IEnumerable<CartItemUpdateDto> CartItems { get; set; }
    }

    public class CartReadUpdateDto : BaseEntity
    {
        public Guid UserId { get; set; }
        public UserReadDto User { get; set; }
        public IEnumerable<CartItemReadDto> CartItems { get; set; }
    }
}