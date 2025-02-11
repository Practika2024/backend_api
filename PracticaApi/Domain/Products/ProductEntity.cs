using Domain.Authentications.Users;
using Domain.ContainerHistories;

namespace Domain.Products;
public class ProductEntity
{
    public ProductId Id { get; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime ManufactureDate { get; private set; }
    public UserEntity? CreatedByNavigation { get; private set; }
    public UserId CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public ProductTypeId? TypeId { get; private set; }
    public ProductTypeEntity? Type { get; private set; }
    public ICollection<ContainerHistoryEntity> Histories { get; private set; } = new List<ContainerHistoryEntity>();

    private ProductEntity(
        ProductId id,
        string name,
        string? description,
        DateTime manufactureDate,
        UserId createdBy,
        ProductTypeId typeId)
    {
        Id = id;
        Name = name;
        Description = description;
        ManufactureDate = manufactureDate;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        TypeId = typeId;
    }

    public static ProductEntity New(
        ProductId id,
        string name,
        string? description,
        DateTime manufactureDate,
        UserId createdBy,
        ProductTypeId typeId)
        => new(id, name, description, manufactureDate, createdBy, typeId);

    public void Update(
        string name,
        string? description,
        DateTime manufactureDate,
        UserId modifiedBy,
        ProductTypeId typeId)
    {
        Name = name;
        Description = description;
        ManufactureDate = manufactureDate;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
        TypeId = typeId;
    }
}

public record ProductId(Guid Value)
{
    public static ProductId New() => new(Guid.NewGuid());
    public static ProductId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}

public enum ProductType
{
    Liquid,
    Solid,
    Powder,
    Other
}