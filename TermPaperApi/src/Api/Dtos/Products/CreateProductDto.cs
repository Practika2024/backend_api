namespace Api.Dtos.Products
{
    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime ManufactureDate { get; set; }
        public Guid TypeId { get; set; }
    }
}