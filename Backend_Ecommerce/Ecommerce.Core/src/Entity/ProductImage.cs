using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.src.Entity
{
    public class ProductImage : BaseEntity
    {
        public string ImageData { get; set; }
        
        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; }
    }
}