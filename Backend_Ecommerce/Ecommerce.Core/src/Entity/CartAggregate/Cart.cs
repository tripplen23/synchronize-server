using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Cart : BaseEntity
    {
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public User User { get; set; }
        public IEnumerable<CartItem> CartItems { get; set; }
    }
}