using Domain.Abstractions;
using Domain.ContainerHistories;
using Domain.Interfaces;
using Domain.Products;
using Domain.Users;

namespace Domain.Containers;

public class ContainerEntity : AuditableEntity
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public decimal Volume { get; private set; }
    public string? Notes { get; private set; }
    public ICollection<ContainerHistoryEntity> Histories { get; private set; } = new List<ContainerHistoryEntity>();
    public string UniqueCode { get; private set; }
    public Guid TypeId { get; private set; }
    public ContainerTypeEntity? Type { get; private set; }
    public Guid? ContentId { get; private set; }
    public ContainerContentEntity? Content { get; private set; }

    private ContainerEntity(
        Guid id,
        string name,
        decimal volume,
        string? notes,
        Guid createdBy,
        Guid typeId,
        string uniqueCode) : base(createdBy)
    {
        Id = id;
        Name = name;
        Volume = volume;
        Notes = notes;
        TypeId = typeId;
        UniqueCode = uniqueCode;
    }

    public static ContainerEntity New(
        Guid id,
        string name,
        decimal volume,
        string? notes,
        Guid createdBy,
        Guid typeId,
        string uniqueCode)
        => new(id, name, volume, notes, createdBy, typeId, uniqueCode);

    public void Update(
        string name,
        decimal volume,
        string? notes,
        Guid typeId
        )
    {
        Name = name;
        Volume = volume;
        Notes = notes;
        TypeId = typeId;
    }

    public void SetContent(ContainerContentEntity content, Guid modifiedBy)
    {
        Content = content;
        ContentId = content.Id;
    }

    public void ClearContent(Guid modifiedBy)
    {
        Content = null;
        ContentId = null;
    }
}