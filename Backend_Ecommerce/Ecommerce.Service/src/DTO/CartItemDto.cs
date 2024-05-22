namespace Ecommerce.Service.src.DTO
{
    public class CartItemReadDto
    {
        public Guid CartId { get; set; }
        public ProductReadDto Product { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemCreateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemUpdateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}