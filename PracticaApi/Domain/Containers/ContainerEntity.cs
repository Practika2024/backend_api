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
    public UserEntity? CreatedByNavigation { get; private set; }
    public UserId CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public ICollection<ContainerHistoryEntity> Histories { get; private set; } = new List<ContainerHistoryEntity>();
    public string UniqueCode { get; private set; }
    public ContainerTypeId? TypeId { get; private set; }
    public ContainerTypeEntity? Type { get; private set; }
    public ContentId? ContentId { get; private set; }
    public ContainerContentEntity? Content { get; private set; }

    private ContainerEntity(
        ContainerId id,
        string name,
        decimal volume,
        string? notes,
        UserId createdBy,
        ContainerTypeId typeId,
        string uniqueCode)
    {
        Id = id;
        Name = name;
        Volume = volume;
        Notes = notes;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        TypeId = typeId;
        UniqueCode = uniqueCode;
    }

    public static ContainerEntity New(
        ContainerId id,
        string name,
        decimal volume,
        string? notes,
        UserId createdBy,
        ContainerTypeId typeId,
        string uniqueCode)
        => new(id, name, volume, notes, createdBy, typeId, uniqueCode);

    public void Update(
        string name,
        decimal volume,
        string? notes,
        UserId modifiedBy,
        ContainerTypeId typeId
        )
    {
        Name = name;
        Volume = volume;
        Notes = notes;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
        TypeId = typeId;
    }

    public void SetContent(ContainerContentEntity content, UserId modifiedBy)
    {
        Content = content;
        ContentId = content.Id;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ClearContent(UserId modifiedBy)
    {
        Content = null;
        ContentId = null;
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