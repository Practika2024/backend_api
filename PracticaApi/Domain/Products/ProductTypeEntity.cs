using Domain.Authentications.Users;

namespace Domain.Products;

public class ProductTypeEntity
{
    public ProductTypeId Id { get; private set; }
    public string Name { get; private set; }

    private ProductTypeEntity(ProductTypeId id, string name)
    {
        Id = id;
        Name = name;
    }

    public static ProductTypeEntity New(string name)
    {
        return new ProductTypeEntity(ProductTypeId.New(), name);
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }
}

public record ProductTypeId(Guid Value)
{
    public static ProductTypeId New() => new(Guid.NewGuid());
    public static ProductTypeId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}