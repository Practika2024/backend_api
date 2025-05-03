using Domain.Products;

namespace DataAccessLayer.Entities.Products;

public class ProductImageEntity
{
    public Guid Id { get; set; }
    public ProductEntity? Product { get; set; }
    public Guid ProductId { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
}