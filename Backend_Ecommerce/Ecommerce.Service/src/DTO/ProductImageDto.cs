using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class ProductImageReadDto : BaseEntity
    {
        public string ImageData { get; set; }
    }

    public class ProductImageCreateDto
    {
        public string ImageData { get; set; }
    }

    public class ProductImageUpdateDto
    {
        public string ImageData { get; set; }
    }
}