using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class ProductImageReadDto : BaseEntity
    {
        public string Url { get; set; }
    }

    public class ProductImageCreateDto
    {
        public string Url { get; set; }
    }

    public class ProductImageUpdateDto
    {
        public string Url { get; set; }
    }
}