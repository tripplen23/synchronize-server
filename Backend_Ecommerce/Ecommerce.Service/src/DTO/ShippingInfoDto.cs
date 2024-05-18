using Ecommerce.Core.src.Entity;

namespace Ecommerce.Service.src.DTO
{
    public class ShippingInfoReadDto : BaseEntity
    {
        public Guid OrderId { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingPostCode { get; set; }
        public string ShippingPhone { get; set; }
    }

    public class ShippingInfoCreateDto
    {
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingPostCode { get; set; }
        public string ShippingPhone { get; set; }
    }

    public class ShippingInfoUpdateDto
    {
        public Guid ShippingInfoId { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingCountry { get; set; }
        public string? ShippingPostCode { get; set; }
        public string? ShippingPhone { get; set; }
    }
}