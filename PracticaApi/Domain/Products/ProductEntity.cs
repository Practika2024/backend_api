using Domain.Abstractions;
using Domain.ContainerHistories;
using Domain.Users;

namespace Domain.Products;
internal class ProductEntity : AuditableEntity
{
    public Guid Id { get; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ManufactureDate { get; set; }
    public Guid TypeId { get; set; }
    public ProductTypeEntity? Type { get; set; }
    public ICollection<ContainerHistoryEntity> Histories { get; set; } = new List<ContainerHistoryEntity>();
}

public enum ProductType
{
    Liquid,
    Solid,
    Powder,
    Other
}