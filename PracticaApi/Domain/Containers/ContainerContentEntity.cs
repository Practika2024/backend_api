using Domain.Abstractions;
using Domain.Products;

namespace Domain.Containers;

public class ContainerContentEntity : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid? ProductId { get; private set; }
    public ProductEntity? Product { get; private set; }
    public bool IsEmpty { get; private set; }

    private ContainerContentEntity(
        Guid id,
        Guid? productId,
        bool isEmpty,
        Guid createdBy) : base(createdBy)
    {
        Id = id;
        ProductId = productId;
        IsEmpty = isEmpty;
    }

    public static ContainerContentEntity New(Guid? productId, bool isEmpty, Guid createdBy)
    {
        return new ContainerContentEntity(Guid.NewGuid(), productId, isEmpty, createdBy);
    }

    public void Update(Guid? productId, bool isEmpty, Guid modifiedBy)
    {
        ProductId = productId;
        IsEmpty = isEmpty;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ClearContent(Guid modifiedBy)
    {
        ProductId = null;
        IsEmpty = true;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}