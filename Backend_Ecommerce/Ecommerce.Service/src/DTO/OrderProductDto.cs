namespace Ecommerce.Service.src.DTO
{
    public class OrderProductReadDto
    {
        public ProductReadDto Product { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderProductCreateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderProductUpdateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}