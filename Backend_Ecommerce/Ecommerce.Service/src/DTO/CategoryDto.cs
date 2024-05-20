namespace Ecommerce.Service.src.DTO
{
    public class CategoryReadDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
    }

    public class CategoryCreateDto
    {
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
    }

    public class CategoryUpdateDto
    {
        public string? CategoryName { get; set; }
        public string? CategoryImage { get; set; }
    }
}