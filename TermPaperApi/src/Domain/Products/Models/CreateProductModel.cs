namespace Domain.Products.Models;

public class CreateProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ManufactureDate { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid TypeId { get; set; }
}