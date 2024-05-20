using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class Product : BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("CategoryId")]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public int Inventory { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}