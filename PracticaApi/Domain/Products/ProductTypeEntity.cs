using Domain.Abstractions;

namespace Domain.Products;

public class ProductTypeEntity : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private ProductTypeEntity(Guid id, string name, Guid createdBy) : base(createdBy)
    {
        Id = id;
        Name = name;
    }

    public static ProductTypeEntity New(string name, Guid createdBy)
    {
        return new ProductTypeEntity(Guid.NewGuid(), name, createdBy);
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }
}