namespace Domain.Products;

public class ProductImage
{
    public Guid Id { get; set; }
    public Product? Product { get; set; }
    public Guid ProductId { get; set; }
    public string? FilePath { get; set; }
}