using Domain.Authentications.Users;
using Domain.ContainerHistories;
using Domain.Products;

namespace Domain.Containers;
public class ContainerEntity
{
    public ContainerId Id { get; }
    public string Name { get; private set; }
    public decimal Volume { get; private set; }
    public string? Notes { get; private set; }
    public bool IsEmpty { get; private set; }
    public ProductId? CurrentProductId { get; private set; }
    public ProductEntity? CurrentProduct { get; private set; }
    public UserId CreatedBy { get; private set; }
    public UserEntity? CreatedByNavigation { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public ICollection<ContainerHistoryEntity> Histories { get; private set; } = new List<ContainerHistoryEntity>();
    public ContainerType Type { get; private set; } 
    public string UniqueCode { get; private set; } 

    private ContainerEntity(
        ContainerId id,
        string name,
        decimal volume,
        string? notes,
        bool isEmpty,
        UserId createdBy,
        ContainerType type,
        string uniqueCode)
    {
        Id = id;
        Name = name;
        Volume = volume;
        Notes = notes;
        IsEmpty = isEmpty;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        Type = type;
        UniqueCode = uniqueCode;
    }

    public static ContainerEntity New(
        ContainerId id,
        string name,
        decimal volume,
        string? notes,
        bool isEmpty,
        UserId createdBy,
        ContainerType type,
        string uniqueCode)
        => new(id, name, volume, notes, isEmpty, createdBy, type, uniqueCode);

    public void Update(
        string name,
        decimal volume,
        string? notes,
        bool isEmpty,
        UserId modifiedBy,
        ContainerType type,
        string uniqueCode)
    {
        Name = name;
        Volume = volume;
        Notes = notes;
        IsEmpty = isEmpty;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
        Type = type;
        UniqueCode = uniqueCode;
    }

    public void SetCurrentProduct(ProductEntity productEntity, UserId modifiedBy)
    {
        CurrentProduct = productEntity;
        CurrentProductId = productEntity.Id;
        IsEmpty = false;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ClearProduct(UserId modifiedBy)
    {
        CurrentProduct = null;
        CurrentProductId = null;
        IsEmpty = true;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}

public record ContainerId(Guid Value)
{
    public static ContainerId New() => new(Guid.NewGuid());
    public static ContainerId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}

public enum ContainerType
{
    Plastic,
    Glass,
    Metal,
    Other
}