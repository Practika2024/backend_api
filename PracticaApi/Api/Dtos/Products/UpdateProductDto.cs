namespace Api.Dtos.Products;

public class UpdateProductDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ManufactureDate { get; set; }
    public Guid TypeId { get; set; }
}