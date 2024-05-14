namespace Ecommerce.Service.src.DTO
{
    public class CartItemReadDto
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductTitle { get; set; }
        public decimal ProductPrice { get; set; }
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