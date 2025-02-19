using Domain.Abstractions;
using Domain.ContainerHistories;
using Domain.Users;

namespace Domain.Products;
public class ProductEntity : AuditableEntity
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime ManufactureDate { get; private set; }
    public Guid TypeId { get; private set; }
    public ProductTypeEntity? Type { get; private set; }
    public ICollection<ContainerHistoryEntity> Histories { get; private set; } = new List<ContainerHistoryEntity>();

    private ProductEntity(
        Guid id,
        string name,
        string? description,
        DateTime manufactureDate,
        Guid createdBy,
        Guid typeId) : base(createdBy)
    {
        Id = id;
        Name = name;
        Description = description;
        ManufactureDate = manufactureDate;
        TypeId = typeId;
    }

    public static ProductEntity New(
        Guid id,
        string name,
        string? description,
        DateTime manufactureDate,
        Guid createdBy,
        Guid typeId)
        => new(id, name, description, manufactureDate, createdBy, typeId);

    public void Update(
        string name,
        string? description,
        DateTime manufactureDate,
        Guid modifiedBy,
        Guid typeId)
    {
        Name = name;
        Description = description;
        ManufactureDate = manufactureDate;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
        TypeId = typeId;
    }
}

public enum ProductType
{
    Liquid,
    Solid,
    Powder,
    Other
}