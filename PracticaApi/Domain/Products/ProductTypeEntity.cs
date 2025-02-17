using Domain.Interfaces;

namespace Domain.Products;

public class ProductTypeEntity : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private ProductTypeEntity(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static ProductTypeEntity New(string name)
    {
        return new ProductTypeEntity(Guid.NewGuid(), name);
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }

    public Guid CreatedBy { get; }
    public DateTime CreatedAt { get; }
    public Guid ModifiedBy { get; }
    public DateTime ModifiedAt { get; }
}