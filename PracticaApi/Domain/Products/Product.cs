using Domain.Authentications.Users;
using Domain.ContainerHistories;

namespace Domain.Products;
public class Product
{
    public ProductId Id { get; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime ManufactureDate { get; private set; }
    public UserId CreatedBy { get; private set; }
    public User? CreatedByNavigation { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public ICollection<ContainerHistory> Histories { get; private set; } = new List<ContainerHistory>();
    public ProductType Type { get; private set; }

    private Product(
        ProductId id,
        string name,
        string? description,
        DateTime manufactureDate,
        UserId createdBy,
        ProductType type)
    {
        Id = id;
        Name = name;
        Description = description;
        ManufactureDate = manufactureDate;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        Type = type;
    }

    public static Product New(
        ProductId id,
        string name,
        string? description,
        DateTime manufactureDate,
        UserId createdBy,
        ProductType type)
        => new(id, name, description, manufactureDate, createdBy, type);

    public void Update(
        string name,
        string? description,
        DateTime manufactureDate,
        UserId modifiedBy,
        ProductType type)
    {
        Name = name;
        Description = description;
        ManufactureDate = manufactureDate;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
        Type = type;
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