using Domain.Products;

namespace DataAccessLayer.Entities.Products;

public class ProductImageEntity
{
    public Guid Id { get; set; }
    public ProductEntity? Product { get; set; }
    public Guid ProductId { get; set; }
    public string? FilePath { get; set; }
}