using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class ProductReadDto : BaseEntity
    {
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductInventory { get; set; }
        public IEnumerable<ProductImageReadDto>? ProductImages { get; set; }
        public Guid CategoryId { get; set; }
        public CategoryReadDto Category { get; set; }
    }

    public class ProductCreateDto
    {
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public Guid CategoryId { get; set; }
        public int ProductInventory { get; set; }
        public IEnumerable<ProductImageCreateDto> ProductImages { get; set; }
    }

    public class ProductUpdateDto : BaseEntity
    {
        public string? ProductTitle { get; set; }
        public string? ProductDescription { get; set; }
        public decimal? ProductPrice { get; set; }
        public Guid? CategoryId { get; set; }
        public int? ProductInventory { get; set; }
        public IEnumerable<ProductImageUpdateDto>? ImagesToUpdate { get; set; }
    }

    public class ProductReviewReadDto
    {
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public int ProductPrice { get; set; }
        public Guid CategoryId { get; set; }
    }
}