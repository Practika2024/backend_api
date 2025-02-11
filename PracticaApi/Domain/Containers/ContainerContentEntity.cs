using Domain.Authentications.Users;
using Domain.Products;

namespace Domain.Containers;

public class ContainerContentEntity
{
    public ContentId Id { get; private set; }
    public ProductId? ProductId { get; private set; }
    public ProductEntity? Product { get; private set; }
    public bool IsEmpty { get; private set; }
    public UserId? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    private ContainerContentEntity(
        ContentId id,
        ProductId? productId,
        bool isEmpty,
        UserId? modifiedBy)
    {
        Id = id;
        ProductId = productId;
        IsEmpty = isEmpty;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public static ContainerContentEntity New(ProductId? productId, bool isEmpty, UserId? modifiedBy)
    {
        return new ContainerContentEntity(ContentId.New(), productId, isEmpty, modifiedBy);
    }

    public void Update(ProductId? productId, bool isEmpty, UserId modifiedBy)
    {
        ProductId = productId;
        IsEmpty = isEmpty;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ClearContent(UserId modifiedBy)
    {
        ProductId = null;
        IsEmpty = true;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}

public record ContentId(Guid Value)
{
    public static ContentId New() => new(Guid.NewGuid());
    public static ContentId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}